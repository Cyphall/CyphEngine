#version 460 core
#extension GL_ARB_bindless_texture : enable

// ---------- DEFINITIONS ----------

struct Uniforms
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

// ---------- INPUTS ----------

layout(location = 0) in vec2 i_position;
layout(location = 1) in vec2 i_uv;

// ---------- UNIFORMS ----------

layout(std430, binding = 0) buffer _0
{
	Uniforms uniforms[];
};

// ---------- OUTPUTS ----------

flat out vec4 v_fillColor;
flat out vec4 v_borderColor;
flat out float v_cornerRadius;
flat out vec2 v_rectangleSize;
out vec2 v_uv;
flat out float v_dpiScaling;
flat out float v_borderThickness;

void main()
{
	v_fillColor = uniforms[gl_InstanceID].u_fillColor;
	v_borderColor = uniforms[gl_InstanceID].u_borderColor;
	v_cornerRadius = uniforms[gl_InstanceID].u_cornerRadius;
	v_rectangleSize = uniforms[gl_InstanceID].u_rectangleSize;
	v_uv = i_uv;
	v_dpiScaling = uniforms[gl_InstanceID].u_dpiScaling;
	v_borderThickness = uniforms[gl_InstanceID].u_borderThickness;
	
	gl_Position = uniforms[gl_InstanceID].u_matrix * vec4(i_position, 0, 1);
}