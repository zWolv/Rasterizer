#version 330

// shader input
in vec2 P;						// fragment position in screen space
in vec2 uv;						// interpolated texture coordinates
uniform sampler2D pixels;		// input texture (1st pass render target)
uniform sampler2D lut;          // look up table texture
void vignetteEffect(), chromaticAberrationEffect(), ColorGradingApply(), FindSizes(float textureSize); // different post processing effects
bool vignette = false, chromaticAberration = false, colorGrading = false;

float size, sizeRoot;

// shader output
out vec3 outputColor;

void main()
{
	// retrieve input pixel
	outputColor = texture( pixels, uv ).rgb;

    FindSizes(textureSize(lut,0).x);

    if (colorGrading)
        ColorGradingApply();

    if (chromaticAberration)
        chromaticAberrationEffect();
    
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

void chromaticAberrationEffect()
{
    vec3 newColor;
    newColor.r = texture( pixels, vec2(uv.x - 0.01, uv.y + 0.01) ).r;
    newColor.g = texture( pixels, vec2(uv.x + 0.01, uv.y - 0.01) ).g;
    newColor.b = texture( pixels, vec2(uv.x - 0.01, uv.y + 0.01) ).b;

//    outputColor *= newColor;
    outputColor = mix(outputColor, newColor, 0.5);
}

void ColorGradingApply()
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

    outputColor = result.rgb;
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

// EOF