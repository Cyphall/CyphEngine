using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace CyphEngine.Rendering;

public class ShaderPipeline : IDisposable
{
	private uint _handle;

	public ShaderPipeline(string name, Dictionary<ShaderType, string> shadersInfo)
	{
		_handle = (uint)GL.CreateProgram();
		
		List<uint> shaders = new List<uint>();
		foreach ((ShaderType type, string sources) in shadersInfo)
		{
			uint shader = LoadShader(type, name, sources);
			GL.AttachShader(_handle, shader);
			shaders.Add(shader);
		}
		
		GL.LinkProgram(_handle);

		for (int i = 0; i < shaders.Count; i++)
		{
			GL.DetachShader(_handle, shaders[i]);
			GL.DeleteShader(shaders[i]);
		}
		
		GL.GetProgram(_handle, GetProgramParameterName.LinkStatus, out int linkSuccess);
	
		if(Convert.ToBoolean(linkSuccess) == false)
		{
			string error = GL.GetProgramInfoLog((int)_handle);
			
			throw new InvalidOperationException($"Error while linking program\n{error}");
		}
	}

	public void Bind()
	{
		GL.UseProgram(_handle);
	}

	public static void Unbind()
	{
		GL.UseProgram(0);
	}

	private static uint LoadShader(ShaderType type, string name, string sources)
	{
		uint handle = (uint)GL.CreateShader(type);

		GL.ShaderSource((int)handle, sources);
		GL.CompileShader(handle);
		
		GL.GetShader(handle, ShaderParameter.CompileStatus, out int compileSuccess);
	
		if(Convert.ToBoolean(compileSuccess) == false)
		{
			string error = GL.GetShaderInfoLog((int)handle);

			throw new InvalidOperationException($"Error while compiling shader \"{name}\" ({type})\n{error}");
		}

		return handle;
	}

	private int GetLocation(string variableName)
	{
		return GL.GetUniformLocation(_handle, variableName);
	}
	
	#region SetValue(float)
	
	public void SetValue(string variableName, params float[] data)
	{
		GL.ProgramUniform1(_handle, GetLocation(variableName), data.Length, data);
	}
	
	public unsafe void SetValue(string variableName, params Vector2[] data)
	{
		fixed (Vector2* ptr = data)
		{
			GL.ProgramUniform2(_handle, GetLocation(variableName), data.Length, (float*)ptr);
		}
	}
	
	public unsafe void SetValue(string variableName, params Vector3[] data)
	{
		fixed (Vector3* ptr = data)
		{
			GL.ProgramUniform3(_handle, GetLocation(variableName), data.Length, (float*)ptr);
		}
	}
	
	public unsafe void SetValue(string variableName, params Vector4[] data)
	{
		fixed (Vector4* ptr = data)
		{
			GL.ProgramUniform4(_handle, GetLocation(variableName), data.Length, (float*)ptr);
		}
	}
	
	public unsafe void SetValue(string variableName, params Matrix2[] data)
	{
		fixed (Matrix2* ptr = data)
		{
			GL.ProgramUniformMatrix2(_handle, GetLocation(variableName), data.Length, false, (float*)ptr);
		}
	}
	
	public unsafe void SetValue(string variableName, params Matrix3[] data)
	{
		fixed (Matrix3* ptr = data)
		{
			GL.ProgramUniformMatrix3(_handle, GetLocation(variableName), data.Length, false, (float*)ptr);
		}
	}
	
	public unsafe void SetValue(string variableName, params Matrix4[] data)
	{
		fixed (Matrix4* ptr = data)
		{
			GL.ProgramUniformMatrix4(_handle, GetLocation(variableName), data.Length, false, (float*)ptr);
		}
	}
	
	#endregion
	#region SetValue(int)
	
	public void SetValue(string variableName, params int[] data)
	{
		GL.ProgramUniform1(_handle, GetLocation(variableName), data.Length, data);
	}
	
	public unsafe void SetValue(string variableName, params Vector2i[] data)
	{
		fixed (Vector2i* ptr = data)
		{
			GL.ProgramUniform2(_handle, GetLocation(variableName), data.Length, (int*)ptr);
		}
	}
	
	public unsafe void SetValue(string variableName, params Vector3i[] data)
	{
		fixed (Vector3i* ptr = data)
		{
			GL.ProgramUniform3(_handle, GetLocation(variableName), data.Length, (int*)ptr);
		}
	}
	
	public unsafe void SetValue(string variableName, params Vector4i[] data)
	{
		fixed (Vector4i* ptr = data)
		{
			GL.ProgramUniform4(_handle, GetLocation(variableName), data.Length, (int*)ptr);
		}
	}
	
	#endregion
	#region SetValue(ulong)
	
	public void SetValue(string variableName, params ulong[] data)
	{
		GL.Arb.ProgramUniformHandle(_handle, GetLocation(variableName), data.Length, data);
	}
	
	#endregion
	
	public void Dispose()
	{
		GL.DeleteProgram(_handle);
	}
}