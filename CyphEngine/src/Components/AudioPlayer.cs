using CyphEngine.Audio;
using JetBrains.Annotations;
using OpenTK.Audio.OpenAL;
using OpenTK.Mathematics;

namespace CyphEngine.Components;

[PublicAPI]
public class AudioPlayer : AComponent
{
	private Sound? _sound;
	private AudioSource _audioSource = new AudioSource();
	private Vector2 _previousPosition;

	public bool Loop
	{
		get => _audioSource.Loop;
		set => _audioSource.Loop = value;
	}

	protected internal override void OnCreate()
	{
		_previousPosition = Transform.WorldPosition;
	}

	protected internal override void OnUpdate(float deltaTime)
	{
		_audioSource.Position = _previousPosition;
		_audioSource.Velocity = Transform.WorldPosition - _previousPosition;
	}

	protected internal override void OnDestroy()
	{
		_audioSource.Dispose();
	}

	public void LoadSound(string filePath)
	{
		_sound = Scene.ResourceManager.RequestSound(filePath);

		_audioSource.Buffer = _sound?.Handle ?? 0;
	}

	public void Start()
	{
		_audioSource.Start();
	}

	public void Stop()
	{
		_audioSource.Pause();
	}

	public void Reset()
	{
		_audioSource.Rewind();
	}

	public bool Playing => _audioSource.State == ALSourceState.Playing;
}