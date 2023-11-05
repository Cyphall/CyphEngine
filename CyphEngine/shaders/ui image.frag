#version 460 core
#extension GL_ARB_bindless_texture : enable

// ---------- INPUTS ----------

in vec2 v_uv;
flat in sampler2D v_texture;
flat in vec4 v_colorMask;

// ---------- OUTPUTS ----------

layout(location = 0) out vec4 o_color;

void main()
{
	o_color = texture(v_texture, v_uv) * v_colorMask;

	if (o_color.a < 0.1f)
	{
		discard;
	}
}