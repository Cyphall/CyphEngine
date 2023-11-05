#version 460 core
#extension GL_ARB_bindless_texture : enable

// ---------- DEFINITIONS ----------

struct Uniforms
{
	layout(bindless_sampler) sampler2D u_texture;
	vec2 _padding0;
	mat4 u_matrix;
	vec4 u_colorMask;
	vec2 u_minUV;
	vec2 u_maxUV;
};

// ---------- INPUTS ----------

layout(location = 0) in vec2 i_position;
layout(location = 1) in vec2 i_uv;

// ---------- UNIFORMS ----------

layout(std430, binding = 0) buffer _0
{
	Uniforms uniforms[];
};

// ---------- OUTPUTS ----------

out vec2 v_uv;
flat out sampler2D v_texture;
flat out vec4 v_colorMask;

void main()
{
	v_uv = mix(uniforms[gl_InstanceID].u_minUV, uniforms[gl_InstanceID].u_maxUV, i_uv);
	
	v_texture = uniforms[gl_InstanceID].u_texture;
	v_colorMask = uniforms[gl_InstanceID].u_colorMask;
	
	gl_Position = uniforms[gl_InstanceID].u_matrix * vec4(i_position, 0, 1);
}