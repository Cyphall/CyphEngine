#version 460 core
#extension GL_ARB_bindless_texture : enable

// ---------- INPUTS ----------

in vec2 v_uv;

// ---------- UNIFORMS ----------

layout(std430, binding = 0) buffer _0
{
	layout(bindless_sampler) sampler2D u_texture;
	float u_sdfAlpha0Value;
	float u_sdfAlpha1Value;
	mat4 u_matrix;
	vec4 u_colorMask;
	vec2 u_minUV;
	vec2 u_maxUV;
};

// ---------- OUTPUTS ----------

layout(location = 0) out vec4 o_color;

float remap(float value, float fromMin, float fromMax, float toMin, float toMax)
{
	return toMin + (value - fromMin) * (toMax - toMin) / (fromMax - fromMin);
}

void main()
{
	o_color = texture(u_texture, v_uv) * u_colorMask;

	o_color.a = clamp(remap(o_color.a, u_sdfAlpha0Value, u_sdfAlpha1Value, 0.0f, 1.0f), 0.0f, 1.0f);
}