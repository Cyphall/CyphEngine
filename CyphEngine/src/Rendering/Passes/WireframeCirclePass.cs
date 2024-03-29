﻿using System.Runtime.InteropServices;
using CyphEngine.Rendering.Uniforms;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using MathHelper = CyphEngine.Helper.MathHelper;

namespace CyphEngine.Rendering.Passes;

public class WireframeCirclePass
{
	private const int CIRCLE_OUTER_VERTEX_COUNT = 16;
	private const int CIRCLE_VERTEX_COUNT = CIRCLE_OUTER_VERTEX_COUNT + 2;

	private Engine _engine;

	private VertexDescriptor _vertexDescriptor;
	private UniformsBuffer<WireframeCircleUniforms> _uniforms;
	private ConstBuffer<VertexData> _vertices;
	private ShaderPipeline _pipeline;

	public WireframeCirclePass(Engine engine)
	{
		_engine = engine;

		//##################################################
		//#################### PIPELINE ####################
		//##################################################
		
		_pipeline = new ShaderPipeline("wireframe circle", new Dictionary<ShaderType, string>
		{
			{ShaderType.VertexShader, ResourceFiles.wireframe_circle_vert},
			{ShaderType.FragmentShader, ResourceFiles.wireframe_circle_frag}
		});

		//##################################################
		//#################### UNIFORMS ####################
		//##################################################
		
		_uniforms = new UniformsBuffer<WireframeCircleUniforms>();

		//##################################################
		//#################### VERTICES ####################
		//##################################################
		
		VertexData[] data = new VertexData[CIRCLE_VERTEX_COUNT];
		
		data[0].Position = new Vector2(0);

		for (int i = 0; i < CIRCLE_OUTER_VERTEX_COUNT+1; i++)
		{
			data[i + 1].Position = MathHelper.VectorFromAngle(i * (360.0f / CIRCLE_OUTER_VERTEX_COUNT));
		}

		_vertices = new ConstBuffer<VertexData>(data);
		
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
		using DebugGroup debugGroup = new DebugGroup("Wireframe circle pass");

		if (_uniforms.UniformCount == 0)
		{
			return;
		}
		
		_vertexDescriptor.Bind();
		
		_uniforms.Upload();
		GL.BindBufferBase(BufferRangeTarget.ShaderStorageBuffer, 0, _uniforms.Handle);
		
		_pipeline.Bind();
		
		GL.DrawArraysInstanced(PrimitiveType.TriangleFan, 0, CIRCLE_VERTEX_COUNT, _uniforms.UniformCount);

		_uniforms.Clear();
	}

	public void AddRequest(Vector4 color, Matrix4 matrix)
	{
		_uniforms.Add(new WireframeCircleUniforms
		{
			Color = color,
			Matrix = matrix
		});
	}
}