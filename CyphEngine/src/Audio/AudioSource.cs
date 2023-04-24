using OpenTK.Audio.OpenAL;
using OpenTK.Mathematics;

namespace CyphEngine.Audio;

internal class AudioSource : IDisposable
{
	private int _handle;
	public int Handle => _handle;
	
	public bool Loop
	{
		get
		{
			AL.GetSource(_handle, ALSourceb.Looping, out bool loop);
			return loop;
		}
		set => AL.Source(_handle, ALSourceb.Looping, value);
	}
	
	public Vector2 Position
	{
		get
		{
			AL.GetSource(_handle, ALSource3f.Position, out float x, out float y, out _);
			return new Vector2(x, y);
		}
		set => AL.Source(_handle, ALSource3f.Position, value.X, value.Y, 0);
	}
	
	public Vector2 Velocity
	{
		get
		{
			AL.GetSource(_handle, ALSource3f.Velocity, out float x, out float y, out _);
			return new Vector2(x, y);
		}
		set => AL.Source(_handle, ALSource3f.Velocity, value.X, value.Y, 0);
	}

	public AudioSource()
	{
		AL.GenSource(out _handle);
		AL.Source(_handle, ALSourcef.Pitch, 1.0f);
		AL.Source(_handle, ALSourcef.Gain, 1.0f);
		Loop = false;
	}

	public void Dispose()
	{
		AL.DeleteSource(_handle);
	}

	public void Start()
	{
		AL.SourcePlay(_handle);
	}

	public void Pause()
	{
		AL.SourcePause(_handle);
	}

	public void Stop()
	{
		AL.SourceStop(_handle);
	}

	public void Rewind()
	{
		AL.SourceRewind(_handle);
	}

	public int Buffer
	{
		get
		{
			AL.GetSource(_handle, ALGetSourcei.Buffer, out int buffer);
			return buffer;
		}
		set => AL.Source(_handle, ALSourcei.Buffer, value);
	}

	public ALSourceState State => AL.GetSourceState(_handle);
}