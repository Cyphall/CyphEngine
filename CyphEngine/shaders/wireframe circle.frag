#version 460 core

// ---------- INPUTS ----------

flat in vec4 v_color;

// ---------- OUTPUTS ----------

layout(location = 0) out vec4 o_color;

void main()
{
	o_color = v_color;
}