#version 460 core
#extension GL_ARB_bindless_texture : enable

// ---------- DEFINITIONS ----------

float remap(float value, float fromMin, float fromMax, float toMin, float toMax);
float calculatePart(vec2 posInRectangle, vec2 contentRectMin, vec2 contentRectMax, float cornerRadius);
float calculateBorderPart(vec2 posInRectangle);
float calculateInnerPart(vec2 posInRectangle);
vec4 premultiplyAlpha(vec4 color);

// ---------- INPUTS ----------

flat in vec4 v_fillColor;
flat in vec4 v_borderColor;
flat in float v_cornerRadius;
flat in vec2 v_rectangleSize;
in vec2 v_uv;
flat in float v_dpiScaling;
flat in float v_borderThickness;

// ---------- OUTPUTS ----------

layout(location = 0) out vec4 o_color;

void main()
{
	vec2 posInRectangle = v_uv * v_rectangleSize;

	float innerPart = calculateInnerPart(posInRectangle);
	float borderPart = calculateBorderPart(posInRectangle) - innerPart;

	o_color = vec4(0);
	o_color += premultiplyAlpha(v_borderColor) * borderPart;
	o_color += premultiplyAlpha(v_fillColor) * innerPart;
}

float remap(float value, float fromMin, float fromMax, float toMin, float toMax)
{
	return toMin + (value - fromMin) * (toMax - toMin) / (fromMax - fromMin);
}

float calculatePart(vec2 posInRectangle, vec2 contentRectMin, vec2 contentRectMax, float cornerRadius)
{
	vec2 closestPos = clamp(posInRectangle, contentRectMin, contentRectMax);

	float distanceToInnerRect = distance(posInRectangle, closestPos);

	float pixelSize = 1.0f / v_dpiScaling;

	return clamp(remap(distanceToInnerRect, cornerRadius, cornerRadius + pixelSize, 1.0f, 0.0f), 0.0f, 1.0f);
}

float calculateBorderPart(vec2 posInRectangle)
{
	float margin = v_cornerRadius + 0.5f;
	vec2 contentRectMin = vec2(margin);
	vec2 contentRectMax = v_rectangleSize - margin;

	return calculatePart(posInRectangle, contentRectMin, contentRectMax, v_cornerRadius);
}

float calculateInnerPart(vec2 posInRectangle)
{
	float margin = max(v_cornerRadius, v_borderThickness) + 0.5f;
	vec2 contentRectMin = vec2(margin);
	vec2 contentRectMax = v_rectangleSize - margin;

	return calculatePart(posInRectangle, contentRectMin, contentRectMax, max(v_cornerRadius - v_borderThickness, 0));
}

vec4 premultiplyAlpha(vec4 color)
{
	color.rgb *= color.a;
	return color;
}
