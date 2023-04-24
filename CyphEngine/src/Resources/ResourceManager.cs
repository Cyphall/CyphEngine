using System.Runtime.InteropServices;
using CyphEngine.Audio;
using JetBrains.Annotations;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace CyphEngine.Resources;

[PublicAPI]
public sealed class ResourceManager : IDisposable
{
	private Dictionary<Tuple<string, bool>, Texture> _imageTextures = new Dictionary<Tuple<string, bool>, Texture>();
	private Dictionary<Vector4i, Texture> _colorTextures = new Dictionary<Vector4i, Texture>();
	private Dictionary<string, Font> _fonts = new Dictionary<string, Font>();
	private Dictionary<string, Sound> _sounds = new Dictionary<string, Sound>();
	
	internal ResourceManager()
	{
		
	}

	public void Dispose()
	{
		foreach (Texture texture in _imageTextures.Values)
		{
			texture.Dispose();
		}
		foreach (Texture texture in _colorTextures.Values)
		{
			texture.Dispose();
		}
		foreach (Font font in _fonts.Values)
		{
			font.Dispose();
		}
		foreach (Sound sound in _sounds.Values)
		{
			sound.Dispose();
		}
	}

	public Texture? RequestImageTexture(string path, bool linearFiltering)
	{
		Tuple<string, bool> key = new Tuple<string, bool>(path, linearFiltering);
		if (!_imageTextures.TryGetValue(key, out Texture? texture))
		{
			texture = Texture.FromFile(path, linearFiltering);

			if (texture == null)
			{
				Console.Error.WriteLine($"Unable to find or read texture file {path}.");
				return null;
			}
			
			_imageTextures.Add(key, texture);
		}

		return texture;
	}

	public Texture RequestColorTexture(Vector4 color)
	{
		Vector4i colorBytes = NormalizedColorToByteColor(color);
		
		if (!_colorTextures.TryGetValue(colorBytes, out Texture? texture))
		{
			byte[] colorData =
			{
				(byte)colorBytes[0],
				(byte)colorBytes[1],
				(byte)colorBytes[2],
				(byte)colorBytes[3]
			};
			
			texture = new Texture(new Vector2i(1, 1), SizedInternalFormat.Rgba8, new Swizzle
			{
				Red = SwizzleSource.Red,
				Green = SwizzleSource.Green,
				Blue = SwizzleSource.Blue,
				Alpha = SwizzleSource.Alpha
			}, false);
			
			GCHandle pinnedArray = GCHandle.Alloc(colorData, GCHandleType.Pinned);
			texture.UploadData(PixelFormat.Rgba, pinnedArray.AddrOfPinnedObject());
			pinnedArray.Free();
			
			_colorTextures.Add(colorBytes, texture);
		}

		return texture;
	}

	public Font? RequestFont(string path)
	{
		if (!_fonts.TryGetValue(path, out Font? font))
		{
			font = Font.FromFile(path);

			if (font == null)
			{
				Console.Error.WriteLine($"Unable to find or read font file {path}.");
				return null;
			}
			
			_fonts.Add(path, font);
		}

		return font;
	}

	public Sound? RequestSound(string path)
	{
		if (!_sounds.TryGetValue(path, out Sound? sound))
		{
			sound = Sound.FromFile(path);

			if (sound == null)
			{
				Console.Error.WriteLine($"Unable to find or read sound file {path}.");
				return null;
			}
			
			_sounds.Add(path, sound);
		}

		return sound;
	}

	private static Vector4i NormalizedColorToByteColor(Vector4 colorFloat)
	{
		Vector4i res = default;
		
		for (int i = 0; i < 4; i++)
		{
			float clampedColor = Math.Clamp(colorFloat[i], 0.0f, 1.0f);

			res[i] = (byte)Math.Floor(clampedColor == 1.0f ? 255.0f : clampedColor * 256.0f);
		}

		return res;
	}
}