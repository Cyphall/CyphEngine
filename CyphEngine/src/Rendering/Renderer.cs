using CyphEngine.Maths;
using CyphEngine.Rendering.Passes;
using CyphEngine.Resources;
using JetBrains.Annotations;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace CyphEngine.Rendering;

[PublicAPI]
public sealed class Renderer : IDisposable
{
	private Engine _engine;

	private SpritePass _spritePass;
	private UIPass _uiPass;
	private WireframeBoxPass _wireframeBoxPass;
	private WireframeCirclePass _wireframeCirclePass;

	public Renderer(Engine engine)
	{
		_engine = engine;

		_spritePass = new SpritePass(engine);
		_uiPass = new UIPass(engine);
		_wireframeBoxPass = new WireframeBoxPass(engine);
		_wireframeCirclePass = new WireframeCirclePass(engine);
		
		GL.ClearColor(0, 0, 0, 1);
		
		GL.Enable(EnableCap.Blend);
		GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
		GL.BlendEquationSeparate(BlendEquationMode.FuncAdd, BlendEquationMode.FuncAdd);
		
		GL.PixelStore(PixelStoreParameter.UnpackAlignment, 1);
	}
	
	public void Render()
	{
		GL.Clear(ClearBufferMask.ColorBufferBit);

		_spritePass.Render();

		GL.Enable(EnableCap.ScissorTest);

		_uiPass.Render();

		GL.Disable(EnableCap.ScissorTest);

		GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);

		_wireframeBoxPass.Render();

		_wireframeCirclePass.Render();

		GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
	}

	public void AddSpriteRequest(Texture texture, Matrix4 matrix, Vector4 colorMask, Rect uvMinMax, float zOffset)
	{
		_spritePass.AddRequest(texture, matrix, colorMask, uvMinMax, zOffset);
	}

	public void AddUIImageRequest(Texture texture, Matrix4 matrix, Vector4 colorMask, Rect uvMinMax)
	{
		_uiPass.AddImageRequest(texture, matrix, colorMask, uvMinMax);
	}

	public void AddUITextRequest(Texture texture, Matrix4 matrix, Vector4 colorMask, Rect uvMinMax, float sdfAlpha0Value, float sdfAlpha1Value)
	{
		_uiPass.AddTextRequest(texture, matrix, colorMask, uvMinMax, sdfAlpha0Value, sdfAlpha1Value);
	}

	public void AddUIRectangleRequest(Vector4 fillColor, Vector4 borderColor, Matrix4 matrix, float cornerRadius, Vector2 rectangleSize, float borderThickness)
	{
		_uiPass.AddRectangleRequest(fillColor, borderColor, matrix, cornerRadius, rectangleSize, borderThickness);
	}

	public void AddWireframeBoxRequest(Vector4 color, Matrix4 matrix)
	{
		_wireframeBoxPass.AddRequest(color, matrix);
	}

	public void AddWireframeCircleRequest(Vector4 color, Matrix4 matrix)
	{
		_wireframeCirclePass.AddRequest(color, matrix);
	}

	public void PushUIScissorArea(Rect scissorArea)
	{
		_uiPass.PushScissorArea(scissorArea);
	}

	public void PopUIScissorArea()
	{
		_uiPass.PopScissorArea();
	}

	public void Dispose()
	{
		_spritePass.Dispose();
		_uiPass.Dispose();
		_wireframeBoxPass.Dispose();
		_wireframeCirclePass.Dispose();
	}
}