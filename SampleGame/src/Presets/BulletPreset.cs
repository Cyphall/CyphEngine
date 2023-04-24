using CyphEngine.Components;
using CyphEngine.Entities;
using OpenTK.Mathematics;

namespace SampleGame;

public class BulletPreset : IEntityPreset
{
	private Vector2 _playerPos;
	
	public BulletPreset(Vector2 playerPos)
	{
		_playerPos = playerPos;
	}
	
	void IEntityPreset.OnApply(Entity entity)
	{
		SpriteRenderer spriteRenderer = entity.CreateComponent<SpriteRenderer>();
		spriteRenderer.LoadTexture("assets/sprites/bullet.png");

		entity.CreateComponent<BulletScript>();

		entity.Transform.LocalPosition = _playerPos;
		
		PhysicsCollider physicsCollider = entity.CreateComponent<PhysicsCollider>();
		physicsCollider.Collider = new BoxCollider(spriteRenderer.Size);
		physicsCollider.LayerName = "playerbullet";
	}
}