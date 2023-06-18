#version 330
 
// shader input
in vec4 position;           // position in world space
in vec2 uv;			        // interpolated texture coordinates
in vec4 normal;		        // interpolated normal
uniform sampler2D pixels;	// texture sampler
uniform vec3 ambientLight;
uniform vec3 lightColor;
uniform vec3 lightPosition;
uniform vec3 cameraPosition;

// shader output
out vec4 outputColor;

// fragment shader
void main()
{
    vec3 L = lightPosition - position.xyz;                          // vector from surface to light, unnormalized!
    vec3 lightToPos = position.xyz - lightPosition;
    vec3 R = normalize(lightToPos - 2 * (dot(lightToPos,normal.xyz) * normal.xyz));
    vec3 V = normalize(cameraPosition - lightPosition);
    float attenuation = 1.0 / dot(L,L);                             // distance attenuation
    float NdotL = max(0, dot(normalize(normal.xyz), normalize(L))); // incoming angle attenuation
    vec3 diffuseColor = texture(pixels, uv).rgb;
    vec3 glossyColor = diffuseColor;                                // since there are no specular objects this will do
    vec3 lightDistanceCorrected = lightColor * attenuation;         // texture lookup
    float n = 3;
    float RdotV = max(0, pow(dot(R,V),n));
    outputColor = vec4(lightDistanceCorrected * (diffuseColor * NdotL + glossyColor * RdotV) + ambientLight * diffuseColor, 1.0);
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