#version 460 core
#extension GL_ARB_bindless_texture : enable

// ---------- INPUTS ----------

in vec2 v_uv;

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

layout(location = 0) out vec4 o_color;

void main()
{
	o_color = texture(u_texture, v_uv) * u_colorMask;
}