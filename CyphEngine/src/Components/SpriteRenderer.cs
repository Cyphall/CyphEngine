using CyphEngine.Maths;
using CyphEngine.Rendering;
using CyphEngine.Resources;
using JetBrains.Annotations;
using OpenTK.Mathematics;

namespace CyphEngine.Components;

[PublicAPI]
public class SpriteRenderer : AComponent
{
	private Texture? _texture;
	
	private float _zOffset;
	public float ZOffset
	{
		get => _zOffset;
		set
		{
			_zOffset = value;
			RecalculateSizeMatrix();
		}
	}

	private Vector2 _size;
	public Vector2 Size
	{
		get => _size;
		set
		{
			_size = value;
			RecalculateSizeMatrix();
		}
	}

	public Rect Uv { get; set; } = Rect.FromTwoPoints(new Vector2(0, 0), new Vector2(1, 1));

	private Matrix4 _sizeMatrix;
	
	public Vector4 ColorMask { get; set; } = Vector4.One;

	public SpriteRenderer()
	{
		RecalculateSizeMatrix();
	}
	
	private void RecalculateSizeMatrix()
	{
		_sizeMatrix = Matrix4.CreateScale(Size.X, Size.Y, 1) * Matrix4.CreateTranslation(0, 0, ZOffset);
	}

	public void LoadTexture(string path, bool applySize = true, bool linearFiltering = false)
	{
		_texture = Scene.ResourceManager.RequestImageTexture(path, linearFiltering);
		
		if (_texture != null && applySize)
		{
			Size = _texture.Size;
		}
	}
	
	public void SetTexture(Vector4 color)
	{
		_texture = Scene.ResourceManager.RequestColorTexture(color);
	}

	protected internal override void OnRender(Renderer renderer, ref Matrix4 viewProjection)
	{
		if (_texture == null)
			return;
		
		Matrix4 localToView = _sizeMatrix * Transform.LocalToWorldMatrix * viewProjection;
		
		renderer.AddGameSpriteRequest(_texture, localToView, ColorMask, Uv);
	}
}