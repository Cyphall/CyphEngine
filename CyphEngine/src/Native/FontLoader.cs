using System.Runtime.InteropServices;
using OpenTK.Mathematics;

namespace CyphEngine.Native;

public static class FontLoader
{
	public static IntPtr InitFont(byte[] fileData)
	{
		return InitFontNative(fileData);
	}

	public static void FreeFont(IntPtr context)
	{
		FreeFontNative(context);
	}

	public static int FindGlyphIndex(IntPtr context, int unicodeCodepoint)
	{
		return FindGlyphIndexNative(context, unicodeCodepoint);
	}
	
	public static float ScaleForPixelHeight(IntPtr context, float height)
	{
		return ScaleForPixelHeightNative(context, height);
	}

	public static void GetFontVMetrics(IntPtr context, out int ascent, out int descent, out int lineGap)
	{
		GetFontVMetricsNative(context, out ascent, out descent, out lineGap);
	}

	public static void GetGlyphHMetrics(IntPtr context, int glyphIndex, out int advanceWidth, out int leftSideBearing)
	{
		GetGlyphHMetricsNative(context, glyphIndex, out advanceWidth, out leftSideBearing);
	}

	public static IntPtr GetGlyphSDF(IntPtr context, float scale, int glyph, int padding, byte onedgeValue, float pixelDistScale, out Vector2i size, out Vector2i baselineOffset)
	{
		return GetGlyphSDFNative(context, scale, glyph, padding, onedgeValue, pixelDistScale, out size.X, out size.Y, out baselineOffset.X, out baselineOffset.Y);
	}

	public static void FreeSDF(IntPtr bitmap)
	{
		FreeSDFNative(bitmap);
	}
	
	[DllImport("CyphEngineNative.dll")]
	private static extern IntPtr InitFontNative(byte[] fileData);
	
	[DllImport("CyphEngineNative.dll")]
	private static extern void FreeFontNative(IntPtr context);
	
	[DllImport("CyphEngineNative.dll")]
	private static extern int FindGlyphIndexNative(IntPtr context, int unicodeCodepoint);
	
	[DllImport("CyphEngineNative.dll")]
	private static extern float ScaleForPixelHeightNative(IntPtr context, float height);
	
	[DllImport("CyphEngineNative.dll")]
	private static extern void GetFontVMetricsNative(IntPtr context, out int ascent, out int descent, out int lineGap);
	
	[DllImport("CyphEngineNative.dll")]
	private static extern void GetGlyphHMetricsNative(IntPtr context, int glyphIndex, out int advanceWidth, out int leftSideBearing);

	[DllImport("CyphEngineNative.dll")]
	private static extern IntPtr GetGlyphSDFNative(IntPtr context, float scale, int glyph, int padding, byte onedgeValue, float pixelDistScale, out int width, out int height, out int xoff, out int yoff);
	
	[DllImport("CyphEngineNative.dll")]
	private static extern void FreeSDFNative(IntPtr bitmap);
}