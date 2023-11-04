using System.Runtime.InteropServices;
using CyphEngine.Rendering.Uniforms;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace CyphEngine.Rendering.Passes;

public class SpritePass
{
	private VertexDescriptor _vertexDescriptor;
	private UniformsBuffer<SpriteUniforms> _uniforms;
	private ConstBuffer<VertexData> _vertices;
	private ShaderPipeline _pipeline;

	public SpritePass()
	{
		//##################################################
		//#################### PIPELINE ####################
		//##################################################
		
		_pipeline = new ShaderPipeline("sprite shader", new Dictionary<ShaderType, string>
		{
			{ShaderType.VertexShader, ResourceFiles.sprite_shader_vert},
			{ShaderType.FragmentShader, ResourceFiles.sprite_shader_frag}
		});

		//##################################################
		//#################### UNIFORMS ####################
		//##################################################

		_uniforms = new UniformsBuffer<SpriteUniforms>();

		//##################################################
		//#################### VERTICES ####################
		//##################################################

		_vertices = new ConstBuffer<VertexData>(new[]
		{
			new VertexData
			{
				Position = new Vector2(-0.5f, 0.5f),
				Uv = new Vector2(0, 0)
			},
			new VertexData
			{
				Position = new Vector2(-0.5f, -0.5f),
				Uv = new Vector2(0, 1)
			},
			new VertexData
			{
				Position = new Vector2(0.5f, 0.5f),
				Uv = new Vector2(1, 0)
			},
			new VertexData
			{
				Position = new Vector2(0.5f, -0.5f),
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
		_uniforms.Dispose();
		_vertices.Dispose();
		_pipeline.Dispose();
	}

	public void Render()
	{
		using DebugGroup debugGroup = new DebugGroup("Sprite pass");

		if (_uniforms.UniformCount == 0)
		{
			return;
		}
		
		_vertexDescriptor.Bind();
		
		_uniforms.Upload();
		GL.BindBufferBase(BufferRangeTarget.ShaderStorageBuffer, 0, _uniforms.Handle);
		
		_pipeline.Bind();
		
		GL.DrawArraysInstanced(PrimitiveType.TriangleStrip, 0, 4, _uniforms.UniformCount);

		_uniforms.Clear();
	}

	public void AddRequest(SpriteUniforms request)
	{
		_uniforms.Add(request);
	}
}