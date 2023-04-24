#pragma once
#include <cstdint>

#include <stb_truetype.h>

extern "C"
{
	__declspec(dllexport) stbtt_fontinfo* InitFontNative(const uint8_t* fileData);
	__declspec(dllexport) void FreeFontNative(stbtt_fontinfo* context);
	
	__declspec(dllexport) int FindGlyphIndexNative(const stbtt_fontinfo* context, int unicodeCodepoint);
	
	__declspec(dllexport) float ScaleForPixelHeightNative(const stbtt_fontinfo* context, float height);
	
	__declspec(dllexport) void GetFontVMetricsNative(const stbtt_fontinfo* context, int& ascent, int& descent, int& lineGap);
	__declspec(dllexport) void GetGlyphHMetricsNative(const stbtt_fontinfo* context, int glyphIndex, int& advanceWidth, int& leftSideBearing);
	
	__declspec(dllexport) uint8_t* GetGlyphSDFNative(const stbtt_fontinfo* context, float scale, int glyph, int padding, uint8_t onedgeValue, float pixelDistScale, int& width, int& height, int& xoff, int& yoff);
	__declspec(dllexport) void FreeSDFNative(uint8_t* bitmap);
}
