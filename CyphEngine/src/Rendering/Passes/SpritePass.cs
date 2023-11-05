using System.Runtime.InteropServices;
using CyphEngine.Maths;
using CyphEngine.Rendering.Uniforms;
using CyphEngine.Resources;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace CyphEngine.Rendering.Passes;

public class SpritePass
{
	private struct SpriteRequest
	{
		public Texture Texture;
		public Matrix4 Matrix;
		public Vector4 ColorMask;
		public Vector2 MinUV;
		public Vector2 MaxUV;
		public float ZOffset;
	}

	private Engine _engine;

	private VertexDescriptor _vertexDescriptor;
	private UniformsBuffer<SpriteUniforms> _uniforms;
	private List<SpriteRequest> _requests = new List<SpriteRequest>();
	private ConstBuffer<VertexData> _vertices;
	private ShaderPipeline _pipeline;

	public SpritePass(Engine engine)
	{
		_engine = engine;

		//##################################################
		//#################### PIPELINE ####################
		//##################################################
		
		_pipeline = new ShaderPipeline("sprite", new Dictionary<ShaderType, string>
		{
			{ShaderType.VertexShader, ResourceFiles.sprite_vert},
			{ShaderType.FragmentShader, ResourceFiles.sprite_frag}
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

		if (_requests.Count == 0)
		{
			return;
		}

		_requests.Sort((a, b) => a.ZOffset.CompareTo(b.ZOffset));
		for (int i = 0; i < _requests.Count; i++)
		{
			SpriteRequest request = _requests[i];

			_uniforms.Add(new SpriteUniforms
			{
				Texture = request.Texture.BindlessHandle,
				Matrix = request.Matrix,
				ColorMask = request.ColorMask,
				MinUV = request.MinUV,
				MaxUV = request.MaxUV
			});
		}
		_requests.Clear();
		
		_vertexDescriptor.Bind();
		
		_uniforms.Upload();
		GL.BindBufferBase(BufferRangeTarget.ShaderStorageBuffer, 0, _uniforms.Handle);
		
		_pipeline.Bind();
		
		GL.DrawArraysInstanced(PrimitiveType.TriangleStrip, 0, 4, _uniforms.UniformCount);

		_uniforms.Clear();
	}

	public void AddRequest(Texture texture, Matrix4 matrix, Vector4 colorMask, Rect uvMinMax, float zOffset)
	{
		_requests.Add(new SpriteRequest
		{
			Texture = texture,
			Matrix = matrix,
			ColorMask = colorMask,
			MinUV = uvMinMax.Min,
			MaxUV = uvMinMax.Max,
			ZOffset = zOffset
		});
	}
}