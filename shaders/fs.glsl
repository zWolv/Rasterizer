#version 330
 
// shader input
in vec4 position;           // position in world space
in vec2 uv;			        // interpolated texture coordinates
in vec4 normal;		        // interpolated normal
uniform sampler2D pixels;	// texture sampler
//uniform vec4 ambientLight;
uniform vec3 lightColor;
uniform vec3 lightPosition;

// shader output
out vec4 outputColor;

// fragment shader
void main()
{
    vec3 L = lightPosition - position.xyz;                          // vector from surface to light, unnormalized!
    float attenuation = 1.0 / dot(L,L);                             // distance attenuation
    float NdotL = max(0, dot(normalize(normal.xyz), normalize(L))); // incoming angle attenuation
    vec3 diffuseColor = texture(pixels, uv).rgb;                    // texture lookup
    outputColor = vec4(lightColor * diffuseColor * attenuation * NdotL, 1.0);
    outputColor.r = clamp(outputColor.r, 0, 1);
    outputColor.g = clamp(outputColor.g, 0, 1);
    outputColor.b = clamp(outputColor.b, 0, 1);
    // complete diffuse shading, A = 1.0 is opaque
}



//template fragment shading
//#version 330
// 
// shader input
//in vec2 uv;			         interpolated texture coordinates
//in vec4 normal;			     interpolated normal
//uniform sampler2D pixels;	 texture sampler
//
// shader output
//out vec4 outputColor;
//
// fragment shader
//void main()
//{
//    outputColor = texture( pixels, uv ) + 0.5f * vec4( normal.xyz, 1 );
//}