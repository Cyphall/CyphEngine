using CyphEngine.Maths;
using CyphEngine.Rendering.Passes;
using CyphEngine.Rendering.Uniforms;
using CyphEngine.Resources;
using JetBrains.Annotations;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace CyphEngine.Rendering;

[PublicAPI]
public sealed class Renderer : IDisposable
{
	private Engine _engine;

	private SpritePass _spritePass = new SpritePass();
	private UIPass _uiPass = new UIPass();
	private WireframeBoxPass _wireframeBoxPass = new WireframeBoxPass();
	private WireframeCirclePass _wireframeCirclePass = new WireframeCirclePass();

	public Renderer(Engine engine)
	{
		_engine = engine;
		
		GL.ClearColor(0, 0, 0, 0);
		
		GL.Enable(EnableCap.Blend);
		GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
		GL.BlendEquationSeparate(BlendEquationMode.FuncAdd, BlendEquationMode.FuncAdd);
		
		GL.PixelStore(PixelStoreParameter.UnpackAlignment, 1);
	}
	
	public void Render()
	{
		GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

		GL.Enable(EnableCap.DepthTest);

		_spritePass.Render();
		
		GL.Disable(EnableCap.DepthTest);

		_uiPass.Render();

		GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);

		_wireframeBoxPass.Render();

		_wireframeCirclePass.Render();

		GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
	}

	public void AddGameSpriteRequest(Texture texture, Matrix4 matrix, Vector4 colorMask, Rect uvMinMax)
	{
		_spritePass.AddRequest(new SpriteUniforms
		{
			Texture = texture.BindlessHandle,
			Matrix = matrix,
			ColorMask = colorMask,
			MinUV = uvMinMax.Min,
			MaxUV = uvMinMax.Max
		});
	}

	public void AddUISpriteRequest(Texture texture, Matrix4 matrix, Vector4 colorMask, Rect uvMinMax)
	{
		_uiPass.AddImageRequest(new SpriteUniforms
		{
			Texture = texture.BindlessHandle,
			Matrix = matrix,
			ColorMask = colorMask,
			MinUV = uvMinMax.Min,
			MaxUV = uvMinMax.Max
		});
	}

	public void AddUITextRequest(Texture texture, Matrix4 matrix, Vector4 colorMask, Rect uvMinMax, float sdfAlpha0Value, float sdfAlpha1Value)
	{
		_uiPass.AddTextRequest(new TextUniforms
		{
			Texture = texture.BindlessHandle,
			SDFAlpha0Value = sdfAlpha0Value,
			SDFAlpha1Value = sdfAlpha1Value,
			Matrix = matrix,
			ColorMask = colorMask,
			MinUV = uvMinMax.Min,
			MaxUV = uvMinMax.Max
		});
	}

	public void AddUIRectangleRequest(Vector4 fillColor, Vector4 borderColor, Matrix4 matrix, float cornerRadius, Vector2 rectangleSize, float borderThickness)
	{
		_uiPass.AddRectangleRequest(new RectangleUniforms
		{
			FillColor = fillColor,
			BorderColor = borderColor,
			Matrix = matrix,
			CornerRadius = cornerRadius,
			RectangleSize = rectangleSize,
			DpiScaling = _engine.Window.Scale,
			BorderThickness = borderThickness
		});
	}

	public void AddWireframeBoxRequest(Vector4 color, Matrix4 matrix)
	{
		_wireframeBoxPass.AddRequest(new WireframeUniforms
		{
			Color = color,
			Matrix = matrix
		});
	}

	public void AddWireframeCircleRequest(Vector4 color, Matrix4 matrix)
	{
		_wireframeCirclePass.AddRequest(new WireframeUniforms
		{
			Color = color,
			Matrix = matrix
		});
	}

	public void Dispose()
	{
		_spritePass.Dispose();
		_uiPass.Dispose();
		_wireframeBoxPass.Dispose();
		_wireframeCirclePass.Dispose();
	}
}