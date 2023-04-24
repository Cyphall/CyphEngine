using CyphEngine.Components;
using CyphEngine.Entities;
using Sharpsteroids.Presets.Weapons;
using Sharpsteroids.Scripts;

namespace Sharpsteroids.Presets.Entities.Bullets;

public class SniperBulletPreset : AWeaponBulletPreset
{
	public override void OnApply(Entity entity)
	{
		SpriteRenderer spriteRenderer = entity.CreateComponent<SpriteRenderer>();
		spriteRenderer.LoadTexture("assets/sprites/bullet1.png");

		SniperBulletScript script = entity.CreateComponent<SniperBulletScript>();
		script.Transform.LocalPosition = StartPosition;
		script.Direction = Direction;
		script.Speed = 3000;
		script.DestroyedOnImpact = true;
	}
}