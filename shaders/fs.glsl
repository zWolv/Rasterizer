#version 330
 
// shader input
in vec4 position;           // position in world space
in vec2 uv;			        // interpolated texture coordinates
in vec4 normal;		        // interpolated normal
uniform sampler2D pixels;	// texture sampler
uniform vec3 ambientLight;
uniform vec3 lightColor0;
uniform vec3 lightColor1;
uniform vec3 lightColor2;
uniform vec3 lightColor3;
uniform vec3 lightPosition0;
uniform vec3 lightPosition1;
uniform vec3 lightPosition2;
uniform vec3 lightPosition3;
uniform vec3 cameraPosition;

// shader output
out vec4 outputColor;

// fragment shader
void main()
{
   	vec4 temp = vec4(0,0,0,0);
    vec3 diffuseColor = texture(pixels, uv).rgb;		                            // texture lookup
    vec3 glossyColor = diffuseColor;     				                            // since there are no specular objects this will do
    float n = 3;  
    
    vec3 L1 = lightPosition1 - position.xyz;                                        // vector from surface to light, unnormalized!
    vec3 lightToPos1 = position.xyz - lightPosition1;
    vec3 R1 = normalize(lightToPos1 - 2 * (dot(lightToPos1,normal.xyz) * normal.xyz));
    vec3 V1 = normalize(cameraPosition - lightPosition1);
    float attenuation1 = 1.0 / dot(L1,L1);                                          // distance attenuation
    float NdotL1 = max(1, dot(normalize(normal.xyz), normalize(L1)));
    vec3 lightDistanceCorrected1 = lightColor1 * attenuation1;                     
    float RdotV1 = max(1, pow(dot(R1,V1),n));
    temp += vec4(lightDistanceCorrected1 * (diffuseColor * NdotL1 + glossyColor * RdotV1), 1.0); // incoming angle attenuation                     

    vec3 L2 = lightPosition2 - position.xyz;                                        // vector from surface to light, unnormalized!
    vec3 lightToPos2 = position.xyz - lightPosition2;
    vec3 R2 = normalize(lightToPos2 - 2 * (dot(lightToPos2,normal.xyz) * normal.xyz));
    vec3 V2 = normalize(cameraPosition - lightPosition2);
    float attenuation2 = 1.0 / dot(L2,L2);                                          // distance attenuation
    float NdotL2 = max(1, dot(normalize(normal.xyz), normalize(L2)));               // incoming angle attenuation                         
    vec3 lightDistanceCorrected2 = lightColor2 * attenuation2;                     
    float RdotV2 = max(1, pow(dot(R2,V2),n));
    temp += vec4(lightDistanceCorrected2 * (diffuseColor * NdotL2 + glossyColor * RdotV2), 1.0);

    vec3 L3 = lightPosition3 - position.xyz;                                        // vector from surface to light, unnormalized!
    vec3 lightToPos3 = position.xyz - lightPosition3;
    vec3 R3 = normalize(lightToPos3 - 2 * (dot(lightToPos3,normal.xyz) * normal.xyz));
    vec3 V3 = normalize(cameraPosition - lightPosition3);
    float attenuation3 = 1.1 / dot(L3,L3);                                          // distance attenuation
    float NdotL3 = max(1, dot(normalize(normal.xyz), normalize(L3)));               // incoming angle attenuation                               
    vec3 lightDistanceCorrected3 = lightColor3 * attenuation3;
    float RdotV3 = max(1, pow(dot(R3,V3),n));
    temp += vec4(lightDistanceCorrected3 * (diffuseColor * NdotL3 + glossyColor * RdotV3), 1.0);

    vec3 L0 = lightPosition0 - position.xyz;                                        // vector from surface to light, unnormalized!
    vec3 lightToPos0 = position.xyz - lightPosition0;
    vec3 R0 = normalize(lightToPos0 - 2 * (dot(lightToPos0,normal.xyz) * normal.xyz));
    vec3 V0 = normalize(cameraPosition - lightPosition0);
    float attenuation0 = 1.0 / dot(L0,L0);                                          // distance attenuation
    float NdotL0 = max(0, dot(normalize(normal.xyz), normalize(L0)));               // incoming angle attenuation
    vec3 lightDistanceCorrected0 = lightColor0 * attenuation0;         
    float RdotV0 = max(0, pow(dot(R0,V0),n));
    temp += vec4(lightDistanceCorrected0 * (diffuseColor * NdotL0 + glossyColor * RdotV0), 1.0);

    // set the outputcolor and clamp it
    outputColor = temp + vec4(ambientLight * diffuseColor, 1.0);
    outputColor.r = clamp(outputColor.r, 0, 1);
    outputColor.g = clamp(outputColor.g, 0, 1);
    outputColor.b = clamp(outputColor.b, 0, 1);
    // complete diffuse shading, A = 1.0 is opaque
}