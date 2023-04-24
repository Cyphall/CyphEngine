using CyphEngine.Native;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace CyphEngine.Resources;

public class Font : IDisposable
{
	public struct RenderData
	{
		public Vector2 TopLeft;
		public Vector2 Size;
		public Vector2 UVMin;
		public Vector2 UVMax;
	}

	private struct CharData
	{
		public float Advance;
		
		public Vector2 BaselineOffset;
		
		public Vector2 Size;

		public Vector2 UVMin;
		public Vector2 UVMax;
	}
	
	public const int GLYPH_HEIGHT = 64;
	public const int SDF_PADDING = GLYPH_HEIGHT / 10;
	public const byte SDF_EDGE_VALUE = 128;
	public const float SDF_PIXEL_DIST_SCALE = SDF_EDGE_VALUE / (float)SDF_PADDING;
	
	internal Texture Texture { get; }
	private List<Tuple<char, char>> _requestedRanges = new List<Tuple<char, char>>();
	private Dictionary<char, CharData> _loadedChars = new Dictionary<char, CharData>();

	private float _maxAscent;

	internal Font(byte[] ttfData)
	{
		_requestedRanges.Add(new Tuple<char, char>((char)0x20, (char)0x7F));
		_requestedRanges.Add(new Tuple<char, char>((char)0xA0, (char)0xFF));
		_requestedRanges.Add(new Tuple<char, char>((char)0x100, (char)0x17F));
		_requestedRanges.Add(new Tuple<char, char>((char)0x25A1, (char)0x25A1));

		Texture = new Texture(new Vector2i(1024, 1024), SizedInternalFormat.R8, new Swizzle
		{
			Red = SwizzleSource.One,
			Green = SwizzleSource.One,
			Blue = SwizzleSource.One,
			Alpha = SwizzleSource.Red
		}, true);

		IntPtr context = FontLoader.InitFont(ttfData);
		
		float scale = FontLoader.ScaleForPixelHeight(context, GLYPH_HEIGHT);

		FontLoader.GetFontVMetrics(context, out int ascent, out _, out _);
		_maxAscent = ascent * scale;

		Vector2i cursor = new Vector2i(0, 0);
		
		for (int i = 0; i < _requestedRanges.Count; i++)
		{
			Tuple<char, char> range = _requestedRanges[i];

			for (char c = range.Item1; c <= range.Item2; c++)
			{
				int glyph = FontLoader.FindGlyphIndex(context, c);

				IntPtr data = FontLoader.GetGlyphSDF(context, scale, glyph, SDF_PADDING, SDF_EDGE_VALUE, SDF_PIXEL_DIST_SCALE,
					out Vector2i bitmapSize, out Vector2i baselineOffset);

				if (cursor.X + bitmapSize.X >= Texture.Size.X)
				{
					cursor = new Vector2i(0, cursor.Y + GLYPH_HEIGHT + SDF_PADDING + SDF_PADDING);
				}

				FontLoader.GetGlyphHMetrics(context, glyph, out int advanceInt, out _);
				float advance = advanceInt * scale;

				CharData charData = new CharData();

				charData.Advance = advance;

				if (data != IntPtr.Zero)
				{
					Texture.UploadSubData(PixelFormat.Red, data, cursor, bitmapSize);
					
					charData.BaselineOffset = baselineOffset + new Vector2i(SDF_PADDING, SDF_PADDING);
				
					charData.Size = bitmapSize - new Vector2i(SDF_PADDING, SDF_PADDING) * 2;

					Vector2 bottomLeft = cursor + new Vector2i(SDF_PADDING, SDF_PADDING);
					Vector2 topRight = bottomLeft + charData.Size;
				
					charData.UVMin = new Vector2(bottomLeft.X, topRight.Y) / Texture.Size;
					charData.UVMax = new Vector2(topRight.X, bottomLeft.Y) / Texture.Size;
				}
					
				FontLoader.FreeSDF(data);
				
				_loadedChars[c] = charData;

				cursor.X += bitmapSize.X + 1;
			}
		}
		
		FontLoader.FreeFont(context);
	}

	internal static Font? FromFile(string filePath)
	{
		try
		{
			byte[] data = File.ReadAllBytes(filePath);

			return new Font(data);
		}
		catch (Exception)
		{
			return null;
		}
	}

	internal RenderData[] GetString(string str, float size)
	{
		RenderData[] res = new RenderData[str.Length];

		float scale = size / GLYPH_HEIGHT;
		
		Vector2 cursorPos = new Vector2(0, _maxAscent * scale);
		
		for (int i = 0; i < str.Length; i++)
		{
			if (!_loadedChars.TryGetValue(str[i], out CharData data))
			{
				data = _loadedChars[(char)0x25A1]; // missing character
			}
			
			res[i].TopLeft = cursorPos + data.BaselineOffset * scale;
			res[i].Size = data.Size * scale;
			res[i].UVMin = data.UVMin;
			res[i].UVMax = data.UVMax;

			cursorPos.X += data.Advance * scale;
		}

		return res;
	}

	internal Vector2 GetStringSize(string str, float size)
	{
		float scale = size / GLYPH_HEIGHT;

		float width = 0;
		
		for (int i = 0; i < str.Length; i++)
		{
			if (!_loadedChars.TryGetValue(str[i], out CharData data))
			{
				data = _loadedChars[(char)0x25A1]; // missing character
			}

			width += data.Advance * scale;
		}

		return new Vector2(width, size);
	}

	public void Dispose()
	{
		Texture.Dispose();
	}
}