using CyphEngine.Helper;
using OpenTK.Graphics.OpenGL4;

namespace CyphEngine.Rendering;

public class UniformsBuffer<TUniforms> : IDisposable
	where TUniforms: unmanaged
{
	private List<TUniforms> _localStorage = new List<TUniforms>();
	private int _gpuStorage;
	private int _gpuStorageSize;

	public int UniformCount => _localStorage.Count;
	public int Handle => _gpuStorage;

	public void Add(TUniforms uniforms)
	{
		_localStorage.Add(uniforms);
	}

	public void Upload()
	{
		if (_localStorage.Count == 0)
		{
			return;
		}
		
		if (_localStorage.Count > _gpuStorageSize)
		{
			GL.DeleteBuffer(_gpuStorage);
			GL.CreateBuffers(1, out _gpuStorage);
			GLHelper.NamedBufferStorage(_gpuStorage, _localStorage, BufferStorageFlags.DynamicStorageBit);
			_gpuStorageSize = _localStorage.Count;
		}
		else
		{
			GLHelper.NamedBufferSubData(_gpuStorage, IntPtr.Zero, _localStorage);
		}
	}

	public void Clear()
	{
		_localStorage.Clear();
	}

	public void Dispose()
	{
		GL.DeleteBuffer(_gpuStorage);
	}
}