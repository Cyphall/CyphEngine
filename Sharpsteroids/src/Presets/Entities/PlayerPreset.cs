using CyphEngine.Components;
using CyphEngine.Entities;
using Sharpsteroids.Scripts;
using Sharpsteroids.Scripts.Weapons;

namespace Sharpsteroids.Presets;

public class PlayerEntityPreset : IEntityPreset
{
	void IEntityPreset.OnApply(Entity entity)
	{
		SpriteRenderer body = entity.CreateComponent<SpriteRenderer>();
		body.LoadTexture("assets/sprites/player.png", true, true);
		body.Size *= 0.2f;
		
		SpriteRenderer thruster = entity.CreateComponent<SpriteRenderer>();
		thruster.LoadTexture("assets/sprites/player_thruster.png", true, true);
		thruster.Size *= 0.2f;

		PlayerScript script = entity.CreateComponent<PlayerScript>();
		script.Thruster = thruster;
		
		AudioPlayer fireAudioPlayer = entity.CreateComponent<AudioPlayer>();
		fireAudioPlayer.LoadSound("assets/sounds/fire.wav");
		
		AudioPlayer thrusterAudioPlayer = entity.CreateComponent<AudioPlayer>();
		thrusterAudioPlayer.LoadSound("assets/sounds/thrust.wav");
		script.ThrusterAudioPlayer = thrusterAudioPlayer;

		entity.CreateComponent<BlasterScript>().AudioPlayer = fireAudioPlayer;
		entity.CreateComponent<ShotgunScript>().AudioPlayer = fireAudioPlayer;
		entity.CreateComponent<EnergyCannonScript>().AudioPlayer = fireAudioPlayer;
		entity.CreateComponent<SniperScript>().AudioPlayer = fireAudioPlayer;

		PhysicsCollider collider = entity.CreateComponent<PhysicsCollider>();
		collider.Collider = new BoxCollider(body.Size);
		collider.LayerName = "player";
	}
}