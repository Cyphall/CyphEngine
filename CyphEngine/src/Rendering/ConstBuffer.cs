using System.Runtime.InteropServices;
using OpenTK.Graphics.OpenGL4;

namespace CyphEngine.Rendering;

public class ConstBuffer<TData> : IDisposable
	where TData: unmanaged
{
	private int _handle;
	public int Handle => _handle;
	
	public ConstBuffer(TData[] data)
	{
		GL.CreateBuffers(1, out _handle);

		GL.NamedBufferStorage(_handle, Marshal.SizeOf<TData>() * data.Length, data, BufferStorageFlags.None);
	}

	public void Dispose()
	{
		GL.DeleteBuffer(_handle);
	}
}