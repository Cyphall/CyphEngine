using CyphEngine.Native;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace CyphEngine.Resources;

public struct Swizzle
{
	public SwizzleSource Red;
	public SwizzleSource Green;
	public SwizzleSource Blue;
	public SwizzleSource Alpha;
}

public enum SwizzleSource
{
	Red = 0x1903,
	Green = 0x1904,
	Blue = 0x1905,
	Alpha = 0x1906,
	Zero = 0,
	One = 1
}

public class Texture : IDisposable
{
	public Vector2i Size { get; }

	private uint _handle;
	internal ulong BindlessHandle { get; }
	
	internal Texture(Vector2i size, SizedInternalFormat internalFormat, Swizzle swizzle, bool linearFiltering, Vector4? fillColor = null)
	{
		GL.CreateTextures(TextureTarget.Texture2D, 1, out _handle);

		Size = size;

		if (linearFiltering)
		{
			GL.TextureParameter(_handle, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
			GL.TextureParameter(_handle, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
		}
		else
		{
			GL.TextureParameter(_handle, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
			GL.TextureParameter(_handle, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
		}

		GL.TextureParameter(_handle, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToBorder);
		GL.TextureParameter(_handle, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToBorder);
		float[] borderColor = { 0, 0, 0, 0 };
		GL.TextureParameter(_handle, TextureParameterName.TextureBorderColor, borderColor);
		
		GL.TextureParameter(_handle, TextureParameterName.TextureSwizzleR, (int)swizzle.Red);
		GL.TextureParameter(_handle, TextureParameterName.TextureSwizzleG, (int)swizzle.Green);
		GL.TextureParameter(_handle, TextureParameterName.TextureSwizzleB, (int)swizzle.Blue);
		GL.TextureParameter(_handle, TextureParameterName.TextureSwizzleA, (int)swizzle.Alpha);
		
		GL.TextureStorage2D(_handle, 1, internalFormat, Size.X, Size.Y);

		if (fillColor.HasValue)
		{
			Vector4 color = fillColor.Value;
			GL.ClearTexImage(_handle, 0, PixelFormat.Rgba, PixelType.Float, ref color);
		}

		BindlessHandle = (ulong)GL.Arb.GetTextureHandle(_handle);
		GL.Arb.MakeTextureHandleResident(BindlessHandle);
	}

	public void UploadData(PixelFormat pixelFormat, IntPtr data)
	{
		GL.TextureSubImage2D(_handle, 0, 0, 0, Size.X, Size.Y, pixelFormat, PixelType.UnsignedByte, data);
	}

	public void UploadSubData(PixelFormat pixelFormat, IntPtr data, Vector2i topLeft, Vector2i size)
	{
		if (topLeft.X < 0 || topLeft.X + size.X > Size.X ||
		    topLeft.Y < 0 || topLeft.Y + size.Y > Size.Y)
		{
			throw new ArgumentException("Sub-data rect is out of bounds.");
		}

		if (size == Vector2i.Zero)
		{
			throw new ArgumentException("Sub-data rect size is 0.");
		}
		
		GL.TextureSubImage2D(_handle, 0, topLeft.X, topLeft.Y, size.X, size.Y, pixelFormat, PixelType.UnsignedByte, data);
	}

	internal static Texture? FromFile(string path, bool linearFiltering)
	{
		Vector2i size;
		IntPtr data = ImageLoader.LoadImage(path, out size.X, out size.Y, out ImageLoader.OriginalChannels originalChannels);

		if (data == IntPtr.Zero)
		{
			return null;
		}

		SizedInternalFormat internalFormat;
		PixelFormat pixelFormat;
		Swizzle swizzle;
		switch (originalChannels)
		{
			case ImageLoader.OriginalChannels.Gray:
				internalFormat = SizedInternalFormat.R8;
				pixelFormat = PixelFormat.Red;
				swizzle = new Swizzle
				{
					Red = SwizzleSource.Red,
					Green = SwizzleSource.Red,
					Blue = SwizzleSource.Red,
					Alpha = SwizzleSource.One
				};
				break;
			case ImageLoader.OriginalChannels.GrayAlpha:
				internalFormat = SizedInternalFormat.Rg8;
				pixelFormat = PixelFormat.Rg;
				swizzle = new Swizzle
				{
					Red = SwizzleSource.Red,
					Green = SwizzleSource.Red,
					Blue = SwizzleSource.Red,
					Alpha = SwizzleSource.Green
				};
				break;
			case ImageLoader.OriginalChannels.RGB:
				internalFormat = SizedInternalFormat.Rgb8;
				pixelFormat = PixelFormat.Rgb;
				swizzle = new Swizzle
				{
					Red = SwizzleSource.Red,
					Green = SwizzleSource.Green,
					Blue = SwizzleSource.Blue,
					Alpha = SwizzleSource.One
				};
				break;
			case ImageLoader.OriginalChannels.RGBAlpha:
				internalFormat = SizedInternalFormat.Rgba8;
				pixelFormat = PixelFormat.Rgba;
				swizzle = new Swizzle
				{
					Red = SwizzleSource.Red,
					Green = SwizzleSource.Green,
					Blue = SwizzleSource.Blue,
					Alpha = SwizzleSource.Alpha
				};
				break;
			default:
				throw new InvalidOperationException();
		}

		Texture texture = new Texture(size, internalFormat, swizzle, linearFiltering);
		texture.UploadData(pixelFormat, data);
		
		ImageLoader.FreeImage(data);
		
		return texture;
	}
	
	public void Dispose()
	{
		GL.DeleteTexture(_handle);
	}
}