using Sharpsteroids.Presets.Entities.Bullets;

namespace Sharpsteroids.Scripts.Weapons;

public class SniperScript : AWeaponScript
{
	protected override void OnCreate()
	{
		Type = WeaponType.Sniper;
		Name = "sniper";
		Cooldown = 1.0f;
	}

	protected override void Fire()
	{
		Scene.CreateEntity(new SniperBulletPreset
		{
			StartPosition = Transform.LocalPosition + Transform.Up * 10,
			Direction = Transform.Up
		}, Scene.Root);
	}
}