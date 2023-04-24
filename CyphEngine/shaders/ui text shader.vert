#version 460 core
#extension GL_ARB_bindless_texture : enable

// ---------- DEFINITIONS ----------

struct Uniforms
{
	layout(bindless_sampler) sampler2D u_texture;
	float u_sdfAlpha0Value;
	float u_sdfAlpha1Value;
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
flat out float v_sdfAlpha0Value;
flat out float v_sdfAlpha1Value;

void main()
{
	v_uv = mix(uniforms[gl_InstanceID].u_minUV, uniforms[gl_InstanceID].u_maxUV, i_uv);

	v_sdfAlpha0Value = uniforms[gl_InstanceID].u_sdfAlpha0Value;
	v_sdfAlpha1Value = uniforms[gl_InstanceID].u_sdfAlpha1Value;
	
	v_texture = uniforms[gl_InstanceID].u_texture;
	v_colorMask = uniforms[gl_InstanceID].u_colorMask;
	
	gl_Position = uniforms[gl_InstanceID].u_matrix * vec4(i_position, 0, 1);
}