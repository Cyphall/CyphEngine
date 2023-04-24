#define STB_TRUETYPE_IMPLEMENTATION

#include "FontLoader.h"

stbtt_fontinfo* InitFontNative(const uint8_t* fileData)
{
	stbtt_fontinfo* context = new stbtt_fontinfo();
	
	stbtt_InitFont(context, fileData, 0);

	return context;
}

void FreeFontNative(stbtt_fontinfo* context)
{
	delete context;
}

int FindGlyphIndexNative(const stbtt_fontinfo* context, int unicodeCodepoint)
{
	return stbtt_FindGlyphIndex(context, unicodeCodepoint);
}

float ScaleForPixelHeightNative(const stbtt_fontinfo* context, float height)
{
	return stbtt_ScaleForPixelHeight(context, height);
}

void GetFontVMetricsNative(const stbtt_fontinfo* context, int& ascent, int& descent, int& lineGap)
{
	stbtt_GetFontVMetrics(context, &ascent, &descent, &lineGap);
}

void GetGlyphHMetricsNative(const stbtt_fontinfo* context, int glyphIndex, int& advanceWidth, int& leftSideBearing)
{
	stbtt_GetGlyphHMetrics(context, glyphIndex, &advanceWidth, &leftSideBearing);
}

uint8_t* GetGlyphSDFNative(const stbtt_fontinfo* context, float scale, int glyph, int padding, uint8_t onedgeValue, float pixelDistScale, int& width, int& height, int& xoff, int& yoff)
{
	return stbtt_GetGlyphSDF(context, scale, glyph, padding, onedgeValue, pixelDistScale, &width, &height, &xoff, &yoff);
}

void FreeSDFNative(uint8_t* bitmap)
{
	stbtt_FreeSDF(bitmap, nullptr);
}
