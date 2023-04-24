using System.Runtime.InteropServices;
using OpenTK.Graphics.OpenGL4;

namespace CyphEngine.Helper;

public static class GLHelper
{
	public static unsafe void NamedBufferStorage<T>(int buffer, List<T> data, BufferStorageFlags usage)
		where T: unmanaged
	{
		Span<T> span = CollectionsMarshal.AsSpan(data);
		fixed (T* ptr = span)
		{
			GL.NamedBufferStorage(buffer, span.Length * Marshal.SizeOf<T>(), (IntPtr)ptr, usage);
		}
	}
	
	public static unsafe void NamedBufferSubData<T>(int buffer, IntPtr offset, List<T> data)
		where T: unmanaged
	{
		Span<T> span = CollectionsMarshal.AsSpan(data);
		fixed (T* ptr = span)
		{
			GL.NamedBufferSubData(buffer, offset, span.Length * Marshal.SizeOf<T>(), (IntPtr)ptr);
		}
	}
}