using OpenTK.Audio.OpenAL;
using OpenTK.Mathematics;

namespace CyphEngine.Audio;

internal class AudioListener
{
	private static AudioListener? _instance;
	public static AudioListener Instance => _instance ??= new AudioListener();

	private AudioListener()
	{
		
	}
	
	public Vector2 Position
	{
		get
		{
			AL.GetListener(ALListener3f.Position, out float x, out float y, out _);
			return new Vector2(x, y);
		}
		set => AL.Listener(ALListener3f.Position, value.X, value.Y, 0);
	}
	
	public Vector2 Velocity
	{
		get
		{
			AL.GetListener(ALListener3f.Velocity, out float x, out float y, out _);
			return new Vector2(x, y);
		}
		set => AL.Listener(ALListener3f.Velocity, value.X, value.Y, 0);
	}
}