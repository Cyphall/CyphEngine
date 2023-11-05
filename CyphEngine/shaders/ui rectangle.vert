#version 460 core
#extension GL_ARB_bindless_texture : enable

// ---------- INPUTS ----------

layout(location = 0) in vec2 i_position;
layout(location = 1) in vec2 i_uv;

// ---------- UNIFORMS ----------

layout(std430, binding = 0) buffer _0
{
	vec4 u_fillColor;
	vec4 u_borderColor;
	mat4 u_matrix;
	vec2 u_rectangleSize;
	float u_cornerRadius;
	float u_dpiScaling;
	vec3 _padding;
	float u_borderThickness;
};

// ---------- OUTPUTS ----------

out vec2 v_uv;

void main()
{
	v_uv = i_uv;
	
	gl_Position = u_matrix * vec4(i_position, 0, 1);
}