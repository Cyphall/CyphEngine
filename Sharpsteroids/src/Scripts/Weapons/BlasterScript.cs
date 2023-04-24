using Sharpsteroids.Presets.Entities.Bullets;

namespace Sharpsteroids.Scripts.Weapons;

public class BlasterScript : AWeaponScript
{
	protected override void OnCreate()
	{
		Type = WeaponType.Blaster;
		Name = "blaster";
		Cooldown = 0.3f;
	}

	protected override void Fire()
	{
		Scene.CreateEntity(new BlasterBulletPreset
		{
			StartPosition = Transform.LocalPosition + Transform.Up * 10,
			Direction = Transform.Up
		}, Scene.Root);
	}
}