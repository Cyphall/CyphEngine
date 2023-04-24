using OpenTK.Graphics.OpenGL4;

namespace CyphEngine.Rendering;

public class VertexDescriptor : IDisposable
{
	public enum IterationRate
	{
		EachVertex,
		EachInstance
	}
	
	private int _handle;
	public int Handle => _handle;
	
	public VertexDescriptor()
	{
		GL.CreateVertexArrays(1, out _handle);
	}
	
	public void Dispose()
	{
		GL.DeleteVertexArray(_handle);
	}
	
	public void DefineFormatF(int bufferSlot, int attribLocation, int elementCount, VertexAttribType elementType, int offset)
	{
		GL.VertexArrayAttribBinding(_handle, attribLocation, bufferSlot); // sets which buffer slot will be used for this vertex attribute
		GL.EnableVertexArrayAttrib(_handle, attribLocation); // enables this vertex attribute
		GL.VertexArrayAttribFormat(_handle, attribLocation, elementCount, elementType, false, offset); // defines this vertex attribute
	}
	
	public void DefineFormatI(int bufferSlot, int attribLocation, int elementCount, VertexAttribType elementType, int offset)
	{
		GL.VertexArrayAttribBinding(_handle, attribLocation, bufferSlot); // sets which buffer slot will be used for this vertex attribute
		GL.EnableVertexArrayAttrib(_handle, attribLocation); // enables this vertex attribute
		GL.VertexArrayAttribIFormat(_handle, attribLocation, elementCount, elementType, offset); // defines this vertex attribute
	}
	
	public void DefineFormatL(int bufferSlot, int attribLocation, int elementCount, VertexAttribType elementType, int offset)
	{
		GL.VertexArrayAttribBinding(_handle, attribLocation, bufferSlot); // sets which buffer slot will be used for this vertex attribute
		GL.EnableVertexArrayAttrib(_handle, attribLocation); // enables this vertex attribute
		GL.VertexArrayAttribLFormat(_handle, attribLocation, elementCount, elementType, offset); // defines this vertex attribute
	}

	public void BindBuffer(int bufferSlot, int bufferHandle, int offset, int elementSize, IterationRate iterationRate)
	{
		GL.VertexArrayVertexBuffer(_handle, bufferSlot, bufferHandle, (IntPtr)offset, elementSize);
		GL.VertexArrayBindingDivisor(_handle, bufferSlot, iterationRate == IterationRate.EachInstance ? 1 : 0);
	}

	public void Bind()
	{
		GL.BindVertexArray(_handle);
	}
}