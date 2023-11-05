using System.Runtime.InteropServices;
using CyphEngine.Maths;
using CyphEngine.Rendering.Uniforms;
using CyphEngine.Resources;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace CyphEngine.Rendering.Passes;

public class UIPass
{
	private enum RenderType
	{
		None,
		Image,
		Text,
		Rectangle
	}

	private struct RenderRequest
	{
		public RenderType Type;
		public Rect ScissorArea;
	}

	private Engine _engine;
	
	private VertexDescriptor _vertexDescriptor;
	private ConstBuffer<VertexData> _vertices;
	
	private UniformsBuffer<UIImageUniforms> _imageUniforms;
	private UniformsBuffer<UITextUniforms> _textUniforms;
	private UniformsBuffer<UIRectangleUniforms> _rectangleUniforms;

	private Stack<Rect> _scissorAreaStack = new Stack<Rect>();
	private List<RenderRequest> _renderRequests = new List<RenderRequest>();

	private ShaderPipeline _imagePipeline;
	private ShaderPipeline _textPipeline;
	private ShaderPipeline _rectanglePipeline;

	public UIPass(Engine engine)
	{
		_engine = engine;

		_scissorAreaStack.Push(Rect.FromOriginSize(Vector2.Zero, _engine.Window.SimulatedSize));

		//##################################################
		//#################### PIPELINE ####################
		//##################################################
		
		_imagePipeline = new ShaderPipeline("ui image", new Dictionary<ShaderType, string>
		{
			{ShaderType.VertexShader, ResourceFiles.ui_image_vert},
			{ShaderType.FragmentShader, ResourceFiles.ui_image_frag}
		});
		
		_textPipeline = new ShaderPipeline("ui text", new Dictionary<ShaderType, string>
		{
			{ShaderType.VertexShader, ResourceFiles.ui_text_vert},
			{ShaderType.FragmentShader, ResourceFiles.ui_text_frag}
		});
		
		_rectanglePipeline = new ShaderPipeline("ui rectangle", new Dictionary<ShaderType, string>
		{
			{ShaderType.VertexShader, ResourceFiles.ui_rectangle_vert},
			{ShaderType.FragmentShader, ResourceFiles.ui_rectangle_frag}
		});

		//##################################################
		//#################### UNIFORMS ####################
		//##################################################

		_imageUniforms = new UniformsBuffer<UIImageUniforms>();
		_textUniforms = new UniformsBuffer<UITextUniforms>();
		_rectangleUniforms = new UniformsBuffer<UIRectangleUniforms>();

		//##################################################
		//#################### VERTICES ####################
		//##################################################

		_vertices = new ConstBuffer<VertexData>(new[]
		{
			new VertexData
			{
				Position = new Vector2(0, 0),
				Uv = new Vector2(0, 0)
			},
			new VertexData
			{
				Position = new Vector2(0, 1),
				Uv = new Vector2(0, 1)
			},
			new VertexData
			{
				Position = new Vector2(1, 0),
				Uv = new Vector2(1, 0)
			},
			new VertexData
			{
				Position = new Vector2(1, 1),
				Uv = new Vector2(1, 1)
			}
		});
		
		//####################################################
		//#################### DESCRIPTOR ####################
		//####################################################
		
		_vertexDescriptor = new VertexDescriptor();
		
		_vertexDescriptor.DefineFormatF(0, 0, 2, VertexAttribType.Float, (int)Marshal.OffsetOf<VertexData>("Position"));
		_vertexDescriptor.DefineFormatF(0, 1, 2, VertexAttribType.Float, (int)Marshal.OffsetOf<VertexData>("Uv"));
		
		_vertexDescriptor.BindBuffer(0, _vertices.Handle, 0, Marshal.SizeOf<VertexData>(), VertexDescriptor.IterationRate.EachVertex);
	}

	public void Dispose()
	{
		_vertexDescriptor.Dispose();
		_vertices.Dispose();
		_imageUniforms.Dispose();
		_textUniforms.Dispose();
		_rectangleUniforms.Dispose();
		_imagePipeline.Dispose();
		_textPipeline.Dispose();
		_rectanglePipeline.Dispose();
	}

	private void ApplyScissor(Rect scissor)
	{
		Vector2i min = new Vector2i(
			(int)Math.Floor(scissor.Min.X * _engine.Window.Scale),
			(int)Math.Floor(scissor.Min.Y * _engine.Window.Scale)
		);

		Vector2i max = new Vector2i(
			(int)Math.Ceiling(scissor.Max.X * _engine.Window.Scale),
			(int)Math.Ceiling(scissor.Max.Y * _engine.Window.Scale)
		);

		Vector2i size = max - min;

		GL.Scissor(
			min.X,
			_engine.Window.FramebufferSize.Y - size.Y - min.Y,
			size.X,
			size.Y
		);
	}

	public void Render()
	{
		using DebugGroup debugGroup = new DebugGroup("UI pass");

		if (_scissorAreaStack.Count > 1)
			throw new InvalidOperationException("The scissor stack was pushed more times than popped");
		if (_scissorAreaStack.Count < 1)
			throw new InvalidOperationException("The scissor stack was popped more times than pushed");

		_vertexDescriptor.Bind();
		
		_imageUniforms.Upload();
		_textUniforms.Upload();
		_rectangleUniforms.Upload();
		
		int imageUniformSize = Marshal.SizeOf<UIImageUniforms>();
		int textUniformSize = Marshal.SizeOf<UITextUniforms>();
		int rectangleUniformSize = Marshal.SizeOf<UIRectangleUniforms>();

		int currentImageIndex = 0;
		int currentTextIndex = 0;
		int currentRectangleIndex = 0;

		RenderType lastType = RenderType.None;
		Rect lastScissorArea = Rect.FromTwoPoints(Vector2.NegativeInfinity, Vector2.PositiveInfinity);

		for (int i = 0; i < _renderRequests.Count; i++)
		{
			RenderRequest request = _renderRequests[i];

			switch (request.Type)
			{
				case RenderType.Image:
				{
					if (request.ScissorArea.Size.X > 0 && request.ScissorArea.Size.Y > 0)
					{
						if (lastType != RenderType.Image)
						{
							GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
							GL.BlendEquationSeparate(BlendEquationMode.FuncAdd, BlendEquationMode.FuncAdd);

							_imagePipeline.Bind();

							lastType = RenderType.Image;
						}

						if (request.ScissorArea != lastScissorArea)
						{
							ApplyScissor(request.ScissorArea);
							lastScissorArea = request.ScissorArea;
						}

						GL.BindBufferRange(BufferRangeTarget.ShaderStorageBuffer, 0, _imageUniforms.Handle, (IntPtr)(imageUniformSize * currentImageIndex), imageUniformSize);
						GL.DrawArrays(PrimitiveType.TriangleStrip, 0, 4);
					}

					currentImageIndex++;

					break;
				}
				case RenderType.Text:
				{
					if (request.ScissorArea.Size.X > 0 && request.ScissorArea.Size.Y > 0)
					{
						if (lastType != RenderType.Text)
						{
							GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
							GL.BlendEquationSeparate(BlendEquationMode.FuncAdd, BlendEquationMode.FuncAdd);

							_textPipeline.Bind();

							lastType = RenderType.Text;
						}

						if (request.ScissorArea != lastScissorArea)
						{
							ApplyScissor(request.ScissorArea);
							lastScissorArea = request.ScissorArea;
						}

						GL.BindBufferRange(BufferRangeTarget.ShaderStorageBuffer, 0, _textUniforms.Handle, (IntPtr)(textUniformSize * currentTextIndex), textUniformSize);
						GL.DrawArrays(PrimitiveType.TriangleStrip, 0, 4);
					}

					currentTextIndex++;

					break;
				}
				case RenderType.Rectangle:
				{
					if (request.ScissorArea.Size.X > 0 && request.ScissorArea.Size.Y > 0)
					{
						if (lastType != RenderType.Rectangle)
						{
							GL.BlendFunc(BlendingFactor.One, BlendingFactor.OneMinusSrcAlpha);
							GL.BlendEquationSeparate(BlendEquationMode.FuncAdd, BlendEquationMode.FuncAdd);

							_rectanglePipeline.Bind();

							lastType = RenderType.Rectangle;
						}

						if (request.ScissorArea != lastScissorArea)
						{
							ApplyScissor(request.ScissorArea);
							lastScissorArea = request.ScissorArea;
						}

						GL.BindBufferRange(BufferRangeTarget.ShaderStorageBuffer, 0, _rectangleUniforms.Handle, (IntPtr)(rectangleUniformSize * currentRectangleIndex), rectangleUniformSize);
						GL.DrawArrays(PrimitiveType.TriangleStrip, 0, 4);
					}

					currentRectangleIndex++;

					break;
				}
			}
		}

		GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
		GL.BlendEquationSeparate(BlendEquationMode.FuncAdd, BlendEquationMode.FuncAdd);

		_imageUniforms.Clear();
		_textUniforms.Clear();
		_rectangleUniforms.Clear();
		_renderRequests.Clear();
	}

	public void AddImageRequest(Texture texture, Matrix4 matrix, Vector4 colorMask, Rect uvMinMax)
	{
		_imageUniforms.Add(new UIImageUniforms
		{
			Texture = texture.BindlessHandle,
			Matrix = matrix,
			ColorMask = colorMask,
			MinUV = uvMinMax.Min,
			MaxUV = uvMinMax.Max
		});

		_renderRequests.Add(new RenderRequest
		{
			Type = RenderType.Image,
			ScissorArea = _scissorAreaStack.Peek()
		});
	}

	public void AddTextRequest(Texture texture, Matrix4 matrix, Vector4 colorMask, Rect uvMinMax, float sdfAlpha0Value, float sdfAlpha1Value)
	{
		_textUniforms.Add(new UITextUniforms
		{
			Texture = texture.BindlessHandle,
			SDFAlpha0Value = sdfAlpha0Value,
			SDFAlpha1Value = sdfAlpha1Value,
			Matrix = matrix,
			ColorMask = colorMask,
			MinUV = uvMinMax.Min,
			MaxUV = uvMinMax.Max
		});

		_renderRequests.Add(new RenderRequest
		{
			Type = RenderType.Text,
			ScissorArea = _scissorAreaStack.Peek()
		});
	}

	public void AddRectangleRequest(Vector4 fillColor, Vector4 borderColor, Matrix4 matrix, float cornerRadius, Vector2 rectangleSize, float borderThickness)
	{
		_rectangleUniforms.Add(new UIRectangleUniforms
		{
			FillColor = fillColor,
			BorderColor = borderColor,
			Matrix = matrix,
			CornerRadius = cornerRadius,
			RectangleSize = rectangleSize,
			DpiScaling = _engine.Window.Scale,
			BorderThickness = borderThickness
		});

		_renderRequests.Add(new RenderRequest
		{
			Type = RenderType.Rectangle,
			ScissorArea = _scissorAreaStack.Peek()
		});
	}

	public void PushScissorArea(Rect scissorArea)
	{
		Rect currentScissorArea = _scissorAreaStack.Peek();

		_scissorAreaStack.Push(Rect.FromTwoPoints(
			Vector2.ComponentMax(currentScissorArea.Min, scissorArea.Min),
			Vector2.ComponentMin(currentScissorArea.Max, scissorArea.Max)
		));
	}

	public void PopScissorArea()
	{
		_scissorAreaStack.Pop();
	}
}