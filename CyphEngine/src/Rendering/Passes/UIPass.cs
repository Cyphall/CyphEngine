using System.Runtime.InteropServices;
using CyphEngine.Rendering.Uniforms;
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
	
	private VertexDescriptor _vertexDescriptor;
	private ConstBuffer<VertexData> _vertices;
	
	private UniformsBuffer<SpriteUniforms> _imageUniforms;
	private UniformsBuffer<TextUniforms> _textUniforms;
	private UniformsBuffer<RectangleUniforms> _rectangleUniforms;
	
	private ShaderPipeline _imagePipeline;
	private ShaderPipeline _textPipeline;
	private ShaderPipeline _rectanglePipeline;

	private List<(RenderType, int)> _requestOrder = new List<(RenderType, int)>();

	public UIPass()
	{
		//##################################################
		//#################### PIPELINE ####################
		//##################################################
		
		_imagePipeline = new ShaderPipeline("sprite shader", new Dictionary<ShaderType, string>
		{
			{ShaderType.VertexShader, ResourceFiles.sprite_shader_vert},
			{ShaderType.FragmentShader, ResourceFiles.sprite_shader_frag}
		});
		
		_textPipeline = new ShaderPipeline("ui text shader", new Dictionary<ShaderType, string>
		{
			{ShaderType.VertexShader, ResourceFiles.ui_text_shader_vert},
			{ShaderType.FragmentShader, ResourceFiles.ui_text_shader_frag}
		});
		
		_rectanglePipeline = new ShaderPipeline("ui rectangle shader", new Dictionary<ShaderType, string>
		{
			{ShaderType.VertexShader, ResourceFiles.ui_rectangle_shader_vert},
			{ShaderType.FragmentShader, ResourceFiles.ui_rectangle_shader_frag}
		});

		//##################################################
		//#################### UNIFORMS ####################
		//##################################################

		_imageUniforms = new UniformsBuffer<SpriteUniforms>();
		_textUniforms = new UniformsBuffer<TextUniforms>();
		_rectangleUniforms = new UniformsBuffer<RectangleUniforms>();

		//##################################################
		//#################### VERTICES ####################
		//##################################################

		_vertices = new ConstBuffer<VertexData>(new[]
		{
			new VertexData
			{
				Position = new Vector2(0, 0),
				Uv = new Vector2(0, 1)
			},
			new VertexData
			{
				Position = new Vector2(0, 1),
				Uv = new Vector2(0, 0)
			},
			new VertexData
			{
				Position = new Vector2(1, 0),
				Uv = new Vector2(1, 1)
			},
			new VertexData
			{
				Position = new Vector2(1, 1),
				Uv = new Vector2(1, 0)
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
		_vertexDescriptor.Bind();
		
		_imageUniforms.Upload();
		_textUniforms.Upload();
		_rectangleUniforms.Upload();
		
		int spriteUniformSize = Marshal.SizeOf<SpriteUniforms>();
		int textUniformSize = Marshal.SizeOf<TextUniforms>();
		int rectangleUniformSize = Marshal.SizeOf<RectangleUniforms>();

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
					GL.BindBufferRange(BufferRangeTarget.ShaderStorageBuffer, 0, _imageUniforms.Handle, (IntPtr)(spriteUniformSize * currentImageIndex), spriteUniformSize * instanceCount);
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

	public void AddImageRequest(SpriteUniforms request)
	{
		_imageUniforms.Add(request);

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

	public void AddTextRequest(TextUniforms request)
	{
		_textUniforms.Add(request);
		
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

	public void AddRectangleRequest(RectangleUniforms request)
	{
		_rectangleUniforms.Add(request);
		
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