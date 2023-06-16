#version 330

// shader input
in vec2 P;						// fragment position in screen space
in vec2 uv;						// interpolated texture coordinates
uniform sampler2D pixels;		// input texture (1st pass render target)
void vignetteEffect(), chromaticAberrationEffect(); // different post processing effects
bool vignette = false, chromaticAberration = true;

// shader output
out vec3 outputColor;

void main()
{
	// retrieve input pixel
	outputColor = texture( pixels, uv ).rgb;

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
    newColor.r = texture( pixels, new vec2(uv.x - 0.01, uv.y - 0.01) ).r;
    newColor.g = texture( pixels, new vec2(uv.x + 0.01, uv.y - 0.01) ).g;
    newColor.b = texture( pixels, new vec2(uv.x + 0.01, uv.y + 0.01) ).b;

    outputColor *= newColor;
}

// EOF