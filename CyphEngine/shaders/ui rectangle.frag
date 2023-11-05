#version 460 core
#extension GL_ARB_bindless_texture : enable

// ---------- DEFINITIONS ----------

float remap(float value, float fromMin, float fromMax, float toMin, float toMax);
float calculatePart(vec2 posInRectangle, vec2 contentRectMin, vec2 contentRectMax, float cornerRadius);
float calculateBorderPart(vec2 posInRectangle);
float calculateInnerPart(vec2 posInRectangle);
vec4 premultiplyAlpha(vec4 color);

// ---------- INPUTS ----------

in vec2 v_uv;

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

layout(location = 0) out vec4 o_color;

void main()
{
	vec2 posInRectangle = v_uv * u_rectangleSize;

	float innerPart = calculateInnerPart(posInRectangle);
	float borderPart = calculateBorderPart(posInRectangle) - innerPart;

	o_color = vec4(0);
	o_color += premultiplyAlpha(u_borderColor) * borderPart;
	o_color += premultiplyAlpha(u_fillColor) * innerPart;
}

float remap(float value, float fromMin, float fromMax, float toMin, float toMax)
{
	return toMin + (value - fromMin) * (toMax - toMin) / (fromMax - fromMin);
}

float calculatePart(vec2 posInRectangle, vec2 contentRectMin, vec2 contentRectMax, float cornerRadius)
{
	vec2 closestPos = clamp(posInRectangle, contentRectMin, contentRectMax);

	float distanceToInnerRect = distance(posInRectangle, closestPos);

	float pixelSize = 1.0f / u_dpiScaling;

	return clamp(remap(distanceToInnerRect, cornerRadius, cornerRadius + pixelSize, 1.0f, 0.0f), 0.0f, 1.0f);
}

float calculateBorderPart(vec2 posInRectangle)
{
	float margin = u_cornerRadius + 0.5f;
	vec2 contentRectMin = vec2(margin);
	vec2 contentRectMax = u_rectangleSize - margin;

	return calculatePart(posInRectangle, contentRectMin, contentRectMax, u_cornerRadius);
}

float calculateInnerPart(vec2 posInRectangle)
{
	float margin = max(u_cornerRadius, u_borderThickness) + 0.5f;
	vec2 contentRectMin = vec2(margin);
	vec2 contentRectMax = u_rectangleSize - margin;

	return calculatePart(posInRectangle, contentRectMin, contentRectMax, max(u_cornerRadius - u_borderThickness, 0));
}

vec4 premultiplyAlpha(vec4 color)
{
	color.rgb *= color.a;
	return color;
}
