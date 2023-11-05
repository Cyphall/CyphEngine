using System.Runtime.InteropServices;
using CyphEngine.Rendering.Uniforms;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace CyphEngine.Rendering.Passes;

public class WireframeBoxPass
{
	private Engine _engine;

	private VertexDescriptor _vertexDescriptor;
	private UniformsBuffer<WireframeBoxUniforms> _uniforms;
	private ConstBuffer<VertexData> _vertices;
	private ShaderPipeline _pipeline;

	public WireframeBoxPass(Engine engine)
	{
		_engine = engine;

		//##################################################
		//#################### PIPELINE ####################
		//##################################################
		
		_pipeline = new ShaderPipeline("wireframe box", new Dictionary<ShaderType, string>
		{
			{ShaderType.VertexShader, ResourceFiles.wireframe_box_vert},
			{ShaderType.FragmentShader, ResourceFiles.wireframe_box_frag}
		});

		//##################################################
		//#################### UNIFORMS ####################
		//##################################################

		_uniforms = new UniformsBuffer<WireframeBoxUniforms>();

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
		using DebugGroup debugGroup = new DebugGroup("Wireframe box pass");

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

	public void AddRequest(Vector4 color, Matrix4 matrix)
	{
		_uniforms.Add(new WireframeBoxUniforms
		{
			Color = color,
			Matrix = matrix
		});
	}
}