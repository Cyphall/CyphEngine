using CyphEngine.Components;
using CyphEngine.Entities;
using OpenTK.Mathematics;
using Sharpsteroids.Scripts;

namespace Sharpsteroids.Presets;

public class ExplosionPreset : IEntityPreset
{
	private string _explosionSound;
	private Vector2 _position;
	private Vector2 _velocity;
	private int _debrisCount;
	private float _debrisLifetime;

	public ExplosionPreset(string explosionSound, Vector2 position, Vector2 velocity, int debrisCount, float debrisLifetime)
	{
		_explosionSound = explosionSound;
		_position = position;
		_velocity = velocity;
		_debrisCount = debrisCount;
		_debrisLifetime = debrisLifetime;
	}
	
	public void OnApply(Entity entity)
	{
		entity.CreateComponent<ExplosionScript>();
		
		AudioPlayer audioPlayer = entity.CreateComponent<AudioPlayer>();
		audioPlayer.LoadSound(_explosionSound);
		
		for (int i = 0; i < _debrisCount; i++)
		{
			entity.Scene.CreateEntity(new DebrisPreset(_debrisLifetime, _position, _velocity), entity.Scene.Root, "debris");
		}
	}
}