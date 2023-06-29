#version 330

// shader input
in vec2 P;						// fragment position in screen space
in vec2 uv;						// interpolated texture coordinates
uniform sampler2D pixels;		// input texture (1st pass render target)
uniform sampler2D lut;          // look up table texture
void vignetteEffect(), BlurEffect(float sigma, int kernelWidth); // different post processing effects
void FindSizes(float textureSize);
vec4 ColorGradingApply(vec2 uv), chromaticAberrationEffect(vec2 uv), BlurPixel(float middleComponent, vec2 uv, float uvComponent, float sigma);
bool vignette = true, chromaticAberration = false, blur = false, colorGrading = false;
float variableKernelWidth = 2.0;
layout(location = 0) out vec4 fragColor;

float size, sizeRoot;

// shader output
out vec3 outputColor;

void main()
{
	// retrieve input pixel
	outputColor = texture( pixels, uv ).rgb;

    FindSizes(textureSize(lut,0).x);

    if (colorGrading)
        outputColor = ColorGradingApply(uv).rgb;

    if (chromaticAberration)
        outputColor = chromaticAberrationEffect(uv).rgb;

    if (blur)
        BlurEffect(variableKernelWidth, 1);
    
    if (vignette)
        vignetteEffect();
}

void vignetteEffect()
{
    // find point distance to center of the screen
    vec2 middle = vec2(0.5, 0.5);
    float distance = length(uv - middle);

    // vignette effect
    float vignette = smoothstep(0.75, 0.4, distance);

    // apply vignette effect
    outputColor *= vignette;
}

vec4 chromaticAberrationEffect(vec2 uv)
{
    vec4 newColor;

    if(!colorGrading)
    {
    newColor.r = texture( pixels, vec2(uv.x - 0.003, uv.y + 0.003) ).r;
    newColor.g = texture( pixels, vec2(uv.x + 0.003, uv.y - 0.003) ).g;
    newColor.b = texture( pixels, vec2(uv.x - 0.003, uv.y + 0.003) ).b;
    }
    else if (colorGrading)
    {
        newColor.r = (ColorGradingApply(vec2(uv.x-0.003, uv.y+0.003))).r;
        newColor.g = (ColorGradingApply(vec2(uv.x+0.003, uv.y-0.003))).g;
        newColor.b = (ColorGradingApply(vec2(uv.x-0.003, uv.y+0.003))).b;
    }

    return newColor;
}

vec4 ColorGradingApply(vec2 uv)
{   
    vec4 tempColor = vec4((texture( pixels, uv ).rgb),0);
    
    float width = sizeRoot * size;
    float height = (size / sizeRoot) * size;
    vec2 wh = vec2(width, height);

    float red = tempColor.r * (size - 1);
	float redinterpol = fract(red);

	float green = tempColor.g * (size - 1);
	float greeninterpol = fract(green);

	float blue = tempColor.b * (size - 1);
	float blueinterpol = fract(blue);

    float row = trunc(blue / sizeRoot);
    float col = trunc(mod(blue, sizeRoot));

    vec2 blueBaseTable = vec2(col * size, row * size) + 0.5;

    vec4 b0r0g0, b0r1g0, b0r0g1, b0r1g1, b1r0g0, b1r1g0, b1r0g1, b1r1g1;

    float redI = min(red + 1, size - 1);
    float greenI = min(green + 1, size - 1);
    float blueI = min(blue + 1, size - 1);
    
    b0r0g0 = texture(lut, vec2(blueBaseTable.x + red, blueBaseTable.y + green) / wh);
    b0r1g0 = texture(lut, vec2(blueBaseTable.x + redI, blueBaseTable.y + green) / wh);
    b0r0g1 = texture(lut, vec2(blueBaseTable.x + red, blueBaseTable.y + greenI) / wh);
    b0r1g1 = texture(lut, vec2(blueBaseTable.x + redI, blueBaseTable.y + greenI) / wh);

    row = trunc(blueI / sizeRoot);
    col = trunc(mod(blueI, sizeRoot));

    blueBaseTable = vec2(trunc(col * size), trunc(row * size)) + 0.5;

    b1r0g0 = texture(lut, vec2(blueBaseTable.x + red, blueBaseTable.y + green) / wh);
    b1r1g0 = texture(lut, vec2(blueBaseTable.x + redI, blueBaseTable.y + green) / wh);
    b1r0g1 = texture(lut, vec2(blueBaseTable.x + red, blueBaseTable.y + greenI) / wh);
    b1r1g1 = texture(lut, vec2(blueBaseTable.x + redI, blueBaseTable.y + greenI) / wh);
    
    vec4 result = mix(mix(b0r0g0, b0r1g0, 0.5), mix(b0r0g1, b0r1g1, 0.5), 0.5);
    vec4 result2 = mix(mix(b1r0g0, b1r1g0, 0.5), mix(b1r0g1, b1r1g1, 0.5), 0.5);
    
    result = mix(result, result2, 0.5);

    return result;
}

void FindSizes(float textureSize)
{
    if (textureSize == 64)
    {
        size = 16;
        sizeRoot = 4;
    }
    else if (textureSize == 512)
    {
        size = 64;
        sizeRoot = 8;
    }
}

void BlurEffect(float sigma, int kernelWidth) 
{
    vec2 middle = uv;
    vec4 blur = vec4(0,0,0,0);

    for(float i = -0.01 * kernelWidth; i < 0.01 + 0.01 * kernelWidth; i += 0.01)
        blur += BlurPixel(middle.x, vec2(uv.x + i, uv.y), uv.x + i, sigma);
    
    for(float i = -0.01 * kernelWidth; i < 0.01 + 0.01 * kernelWidth; i += 0.01)
        blur += BlurPixel(middle.y, vec2(uv.x, uv.y + i), uv.y + i, sigma);
    normalize(blur);

    outputColor = blur.rgb;
}

vec4 BlurPixel(float middleComponent, vec2 uv, float uvComponent, float sigma)
{
    #define PI 3.1415926538
    
    float num = pow(uvComponent - middleComponent, 2);
    float denum = 2 * (sigma * sigma);
    float div = -(num/denum);

    float e = pow(exp(1.0), div);
    float root = sqrt(2 * PI * pow(sigma,2));

    float result = 1 / root * e;

    vec4 pixel;
    if (colorGrading && !chromaticAberration)
        pixel = ColorGradingApply(uv);
    else if (chromaticAberration)
        pixel = chromaticAberrationEffect(uv);
    else
        pixel = texture(pixels,uv);

    return pixel * result;
}



// EOF