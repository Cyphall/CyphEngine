#version 460 core
#extension GL_ARB_bindless_texture : enable

// ---------- INPUTS ----------

in vec2 v_uv;
flat in sampler2D v_texture;
flat in vec4 v_colorMask;
flat in float v_sdfAlpha0Value;
flat in float v_sdfAlpha1Value;

// ---------- OUTPUTS ----------

layout(location = 0) out vec4 o_color;

float remap(float value, float fromMin, float fromMax, float toMin, float toMax)
{
	return toMin + (value - fromMin) * (toMax - toMin) / (fromMax - fromMin);
}

void main()
{
	o_color = texture(v_texture, v_uv) * v_colorMask;

	o_color.a = clamp(remap(o_color.a, v_sdfAlpha0Value, v_sdfAlpha1Value, 0.0f, 1.0f), 0.0f, 1.0f);
}