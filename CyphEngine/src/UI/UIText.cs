using CyphEngine.Maths;
using CyphEngine.Rendering;
using CyphEngine.Resources;
using JetBrains.Annotations;
using OpenTK.Mathematics;

namespace CyphEngine.UI;

[PublicAPI]
public class UIText : AUIElement
{
	private string _text = "";
	private Font? _font;
	public Vector2 TextSize { get; private set; }
	private float _fontSize = 20;
	
	public string Text
	{
		get => _text;
		set
		{
			_text = value;
			
			RecalculateTextSize();
			Manager.SetDirty();
		}
	}

	public Vector4 ColorMask { get; set; } = Vector4.One;

	public float FontSize
	{
		get => _fontSize;
		set
		{
			_fontSize = value;
			
			RecalculateTextSize();
			Manager.SetDirty();
		}
	}

	public string FontFile
	{
		set
		{
			_font = Engine.ResourceManager.RequestFont(value);
			
			RecalculateTextSize();
			Manager.SetDirty();
		}
	}

	private void RecalculateTextSize()
	{
		if (_font == null)
		{
			TextSize = new Vector2(0);
			return;
		}
		
		if (string.IsNullOrEmpty(_text))
		{
			TextSize = new Vector2(0);
			return;
		}
		
		TextSize = _font.GetStringSize(Text, FontSize);
	}

	protected override Vector2 MeasureOverride(Vector2 availableSize)
	{
		return Vector2.ComponentMin(TextSize, availableSize);
	}

	protected override void RenderOverride(Renderer renderer, ref Matrix4 projection)
	{
		if (_font == null)
			return;

		if (TextSize.X > 0 && TextSize.Y > 0)
		{
			float scale = Font.GLYPH_HEIGHT / _fontSize;
			float distancePerPixel = Font.SDF_PIXEL_DIST_SCALE / 255.0f / Window.Scale;
			float scaledDistancePerPixel = distancePerPixel * scale;

			float edgeValue = Font.SDF_EDGE_VALUE / 255.0f;

			Font.RenderData[] renderData = _font.GetString(Text, FontSize);

			Vector2 origin = ContentPosition;

			for (int i = 0; i < renderData.Length; i++)
			{
				ref Font.RenderData data = ref renderData[i];

				if (data.Size == Vector2.Zero)
				{
					continue;
				}

				Matrix4 matrix = Matrix4.CreateScale(data.Size.X, data.Size.Y, 1) * Matrix4.CreateTranslation(origin.X + data.TopLeft.X, origin.Y + data.TopLeft.Y, 0) * projection;

				renderer.AddUITextRequest(_font.Texture, matrix, ColorMask, Rect.FromTwoPoints(data.UVMin, data.UVMax), edgeValue - scaledDistancePerPixel/2, edgeValue + scaledDistancePerPixel/2);
			}
		}
		
		base.RenderOverride(renderer, ref projection);
	}
}