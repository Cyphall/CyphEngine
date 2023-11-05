#version 460 core
#extension GL_ARB_bindless_texture : enable

// ---------- INPUTS ----------

layout(location = 0) in vec2 i_position;
layout(location = 1) in vec2 i_uv;

// ---------- UNIFORMS ----------

layout(std430, binding = 0) buffer _0
{
	layout(bindless_sampler) sampler2D u_texture;
	vec2 _padding0;
	mat4 u_matrix;
	vec4 u_colorMask;
	vec2 u_minUV;
	vec2 u_maxUV;
};

// ---------- OUTPUTS ----------

out vec2 v_uv;

void main()
{
	v_uv = mix(u_minUV, u_maxUV, i_uv);
	
	gl_Position = u_matrix * vec4(i_position, 0, 1);
}