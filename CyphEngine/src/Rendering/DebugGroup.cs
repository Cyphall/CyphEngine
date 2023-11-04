using OpenTK.Graphics.OpenGL4;

namespace CyphEngine.Rendering;

public struct DebugGroup : IDisposable
{
	public DebugGroup(string name)
	{
		GL.PushDebugGroup(DebugSourceExternal.DebugSourceApplication, 0, name.Length, name);
	}

	public void Dispose()
	{
		GL.PopDebugGroup();
	}
}