using CyphEngine.Components;
using CyphEngine.Entities;
using Sharpsteroids.Presets.Weapons;
using Sharpsteroids.Scripts;

namespace Sharpsteroids.Presets.Entities.Bullets;

public class BlasterBulletPreset : AWeaponBulletPreset
{
	public override void OnApply(Entity entity)
	{
		SpriteRenderer spriteRenderer = entity.CreateComponent<SpriteRenderer>();
		spriteRenderer.LoadTexture("assets/sprites/bullet1.png");

		BulletScript script = entity.CreateComponent<BulletScript>();
		script.Transform.LocalPosition = StartPosition;
		script.Direction = Direction;
		script.Speed = 500;
		script.DestroyedOnImpact = true;

		PhysicsCollider collider = entity.CreateComponent<PhysicsCollider>();
		collider.Collider = new BoxCollider(spriteRenderer.Size);
		collider.LayerName = "bullet";
	}
}