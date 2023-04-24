using CyphEngine.Components;
using CyphEngine.Entities;
using Sharpsteroids.Presets.Weapons;
using Sharpsteroids.Scripts;

namespace Sharpsteroids.Presets.Entities.Bullets;

public class EnergyBallPreset : AWeaponBulletPreset
{
	public override void OnApply(Entity entity)
	{
		SpriteRenderer spriteRenderer = entity.CreateComponent<SpriteRenderer>();
		spriteRenderer.LoadTexture("assets/sprites/bullet2.png");

		BulletScript script = entity.CreateComponent<BulletScript>();
		script.Transform.LocalPosition = StartPosition;
		script.Direction = Direction;
		script.Speed = 200;
		script.DestroyedOnImpact = false;

		PhysicsCollider collider = entity.CreateComponent<PhysicsCollider>();
		collider.Collider = new CircleCollider(7.5f);
		collider.LayerName = "bullet";
	}
}