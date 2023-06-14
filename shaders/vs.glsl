	#version 330
 
// shader input
in vec2 vUV;					// vertex uv coordinate
in vec3 vNormal;				// untransformed vertex normal
in vec3 vPosition;				// untransformed vertex position

// shader output
out vec4 position;				// transformed vertex position	(world space)
out vec4 normal;				// transformed vertex normal	(world space)
out vec2 uv;					// vertex uv coordinate
uniform mat4 objectToScreen;	// transformation matrix from object to screen space
uniform mat4 objectToWorld;		// transformation matrix from object to world space
 
// vertex shader
void main()
{
	// transform vertex using supplied matrix
	gl_Position = objectToScreen * vec4(vPosition, 1.0f);

	// set the world space position
	position = objectToWorld * vec4(vPosition, 1.0f);

	// forward normal and uv coordinate; will be interpolated over triangle
	normal = objectToWorld * vec4( vNormal, 0.0f );
	uv = vUV;
}