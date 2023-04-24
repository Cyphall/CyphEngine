using CyphEngine.Components;
using CyphEngine.Entities;
using OpenTK.Mathematics;

namespace SampleGame;

public class PlayerPreset : IEntityPreset
{
	void IEntityPreset.OnApply(Entity entity)
	{
		SpriteRenderer spriteRenderer = entity.CreateComponent<SpriteRenderer>();
		spriteRenderer.LoadTexture("assets/sprites/player.png");

		entity.CreateComponent<PlayerScript>();

		entity.Transform.LocalPosition = new Vector2(0, -279);
		
		PhysicsCollider physicsCollider = entity.CreateComponent<PhysicsCollider>();
		physicsCollider.Collider = new BoxCollider(spriteRenderer.Size);
		physicsCollider.LayerName = "player";
	}
}