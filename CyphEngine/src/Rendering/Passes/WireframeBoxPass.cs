using System.Runtime.InteropServices;
using CyphEngine.Rendering.Uniforms;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace CyphEngine.Rendering.Passes;

public class WireframeBoxPass
{
	private VertexDescriptor _vertexDescriptor;
	private UniformsBuffer<WireframeUniforms> _uniforms;
	private ConstBuffer<VertexData> _vertices;
	private ShaderPipeline _pipeline;

	public WireframeBoxPass()
	{
		//##################################################
		//#################### PIPELINE ####################
		//##################################################
		
		_pipeline = new ShaderPipeline("wireframe shader", new Dictionary<ShaderType, string>
		{
			{ShaderType.VertexShader, ResourceFiles.wireframe_shader_vert},
			{ShaderType.FragmentShader, ResourceFiles.wireframe_shader_frag}
		});

		//##################################################
		//#################### UNIFORMS ####################
		//##################################################

		_uniforms = new UniformsBuffer<WireframeUniforms>();

		//##################################################
		//#################### VERTICES ####################
		//##################################################

		_vertices = new ConstBuffer<VertexData>(new[]
		{
			new VertexData
			{
				Position = new Vector2(-0.5f, -0.5f)
			},
			new VertexData
			{
				Position = new Vector2(-0.5f, 0.5f)
			},
			new VertexData
			{
				Position = new Vector2(0.5f, -0.5f)
			},
			new VertexData
			{
				Position = new Vector2(0.5f, 0.5f)
			}
		});
		
		//####################################################
		//#################### DESCRIPTOR ####################
		//####################################################
		
		_vertexDescriptor = new VertexDescriptor();
		
		_vertexDescriptor.DefineFormatF(0, 0, 2, VertexAttribType.Float, (int)Marshal.OffsetOf<VertexData>("Position"));
		
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

	public void AddRequest(WireframeUniforms request)
	{
		_uniforms.Add(request);
	}
}