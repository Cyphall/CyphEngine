using System.Runtime.InteropServices;

namespace CyphEngine.Native;

public static class ImageLoader
{
	public enum DesiredChannels
	{
		Default = 0,
		Gray = 1,
		GrayAlpha = 2,
		RGB = 3,
		RGBAlpha = 4
	}

	public enum OriginalChannels
	{
		Gray = 1,
		GrayAlpha = 2,
		RGB = 3,
		RGBAlpha = 4
	}

	public static IntPtr LoadImage(string filename, out int width, out int height, out OriginalChannels originalChannels, DesiredChannels desiredChannels = DesiredChannels.Default)
	{
		IntPtr filenameUTF8 = Marshal.StringToCoTaskMemUTF8(filename);
		IntPtr result = LoadImageNative(filenameUTF8, out width, out height, out originalChannels, desiredChannels);
		Marshal.FreeCoTaskMem(filenameUTF8);
		return result;
	}

	public static void FreeImage(IntPtr image)
	{
		FreeImageNative(image);
	}
	
	[DllImport("CyphEngineNative.dll")]
	private static extern IntPtr LoadImageNative(IntPtr filename, out int width, out int height, out OriginalChannels originalChannels, DesiredChannels desiredChannels);
	
	[DllImport("CyphEngineNative.dll")]
	private static extern void FreeImageNative(IntPtr image);
}