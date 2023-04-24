using CyphEngine.Components;

namespace Sharpsteroids.Scripts;

public class ExplosionScript : AComponent
{
	private AudioPlayer _audioPlayer = null!;
	
	protected override void OnInit()
	{
		_audioPlayer = Entity.GetComponent<AudioPlayer>()!;
		_audioPlayer.Start();
	}

	protected override void OnUpdate(float deltaTime)
	{
		if (!_audioPlayer.Playing)
		{
			Scene.DestroyEntity(Entity);
		}
	}
}