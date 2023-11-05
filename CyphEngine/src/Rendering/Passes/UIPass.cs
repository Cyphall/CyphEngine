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
		Image,
		Text,
		Rectangle
	}

	private Engine _engine;
	
	private VertexDescriptor _vertexDescriptor;
	private ConstBuffer<VertexData> _vertices;
	
	private UniformsBuffer<UIImageUniforms> _imageUniforms;
	private UniformsBuffer<UITextUniforms> _textUniforms;
	private UniformsBuffer<UIRectangleUniforms> _rectangleUniforms;
	
	private ShaderPipeline _imagePipeline;
	private ShaderPipeline _textPipeline;
	private ShaderPipeline _rectanglePipeline;

	private List<(RenderType, int)> _requestOrder = new List<(RenderType, int)>();

	public UIPass(Engine engine)
	{
		_engine = engine;

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

	public void Render()
	{
		using DebugGroup debugGroup = new DebugGroup("UI pass");

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

		for (int i = 0; i < _requestOrder.Count; i++)
		{
			int instanceCount = _requestOrder[i].Item2;
			
			switch (_requestOrder[i].Item1)
			{
				case RenderType.Image:
				{
					_imagePipeline.Bind();
					GL.BindBufferRange(BufferRangeTarget.ShaderStorageBuffer, 0, _imageUniforms.Handle, (IntPtr)(imageUniformSize * currentImageIndex), imageUniformSize * instanceCount);
					GL.DrawArraysInstanced(PrimitiveType.TriangleStrip, 0, 4, instanceCount);
					currentImageIndex += instanceCount;

					break;
				}
				case RenderType.Text:
				{
					_textPipeline.Bind();
					GL.BindBufferRange(BufferRangeTarget.ShaderStorageBuffer, 0, _textUniforms.Handle, (IntPtr)(textUniformSize * currentTextIndex), textUniformSize * instanceCount);
					GL.DrawArraysInstanced(PrimitiveType.TriangleStrip, 0, 4, instanceCount);
					currentTextIndex += instanceCount;

					break;
				}
				case RenderType.Rectangle:
				{
					GL.BlendFunc(BlendingFactor.One, BlendingFactor.OneMinusSrcAlpha);
					GL.BlendEquationSeparate(BlendEquationMode.FuncAdd, BlendEquationMode.FuncAdd);
					
					_rectanglePipeline.Bind();
					GL.BindBufferRange(BufferRangeTarget.ShaderStorageBuffer, 0, _rectangleUniforms.Handle, (IntPtr)(rectangleUniformSize * currentRectangleIndex), rectangleUniformSize * instanceCount);
					GL.DrawArraysInstanced(PrimitiveType.TriangleStrip, 0, 4, instanceCount);
					currentRectangleIndex += instanceCount;
					
					GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
					GL.BlendEquationSeparate(BlendEquationMode.FuncAdd, BlendEquationMode.FuncAdd);

					break;
				}
			}
		}

		_imageUniforms.Clear();
		_textUniforms.Clear();
		_rectangleUniforms.Clear();
		_requestOrder.Clear();
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

		if (_requestOrder.Count == 0 || _requestOrder[^1].Item1 != RenderType.Image)
		{
			_requestOrder.Add((RenderType.Image, 1));
		}
		else
		{
			(RenderType type, int count) = _requestOrder[^1];
			_requestOrder[^1] = (type, count + 1);
		}
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
		
		if (_requestOrder.Count == 0 || _requestOrder[^1].Item1 != RenderType.Text)
		{
			_requestOrder.Add((RenderType.Text, 1));
		}
		else
		{
			(RenderType type, int count) = _requestOrder[^1];
			_requestOrder[^1] = (type, count + 1);
		}
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
		
		if (_requestOrder.Count == 0 || _requestOrder[^1].Item1 != RenderType.Rectangle)
		{
			_requestOrder.Add((RenderType.Rectangle, 1));
		}
		else
		{
			(RenderType type, int count) = _requestOrder[^1];
			_requestOrder[^1] = (type, count + 1);
		}
	}
}