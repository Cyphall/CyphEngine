using CyphEngine.Maths;
using CyphEngine.Rendering;
using CyphEngine.Resources;
using JetBrains.Annotations;
using OpenTK.Mathematics;

namespace CyphEngine.UI;

[PublicAPI]
public class UIImage : AUIElement
{
	private Texture? _texture;
	
	public Vector2i TextureSize { get; private set; } = Vector2i.Zero;
	
	public Vector4 ColorMask { get; set; } = Vector4.One;
	
	public Rect Uv { get; set; } = Rect.FromTwoPoints(new Vector2(0, 0), new Vector2(1, 1));
	
	public void LoadTexture(string path, bool linearFiltering = false)
	{
		_texture = Scene.ResourceManager.RequestImageTexture(path, linearFiltering);

		TextureSize = _texture?.Size ?? Vector2i.Zero;
		
		Manager.SetDirty();
	}
	
	public void SetTexture(Vector4 color)
	{
		_texture = Scene.ResourceManager.RequestColorTexture(color);

		TextureSize = new Vector2i(1, 1);
		
		Manager.SetDirty();
	}
	
	protected override Vector2 MeasureOverride(Vector2 availableSize)
	{
		return Vector2.ComponentMin(_texture?.Size ?? Vector2.Zero, availableSize);
	}
	
	protected override void RenderOverride(Renderer renderer, ref Matrix4 projection)
	{
		if (_texture == null)
			return;
		
		Matrix4 matrix = Matrix4.CreateScale(ActualContentWidth, ActualContentHeight, 1) * Matrix4.CreateTranslation(ContentX, ContentY, 0) * projection;
		
		renderer.AddUISpriteRequest(_texture, matrix, ColorMask, Uv);
		
		base.RenderOverride(renderer, ref projection);
	}
}