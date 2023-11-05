#version 460 core
#extension GL_ARB_bindless_texture : enable

// ---------- DEFINITIONS ----------

struct Uniforms
{
	vec4 u_color;
	mat4 u_matrix;
};

// ---------- INPUTS ----------

layout(location = 0) in vec2 i_position;

// ---------- UNIFORMS ----------

layout(std430, binding = 0) buffer _0
{
	Uniforms uniforms[];
};

// ---------- OUTPUTS ----------

flat out vec4 v_color;

void main()
{
	v_color = uniforms[gl_InstanceID].u_color;
	
	gl_Position = uniforms[gl_InstanceID].u_matrix * vec4(i_position, 0, 1);
}