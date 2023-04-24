using Sharpsteroids.Presets.Entities.Bullets;

namespace Sharpsteroids.Scripts.Weapons;

public class EnergyCannonScript : AWeaponScript
{
	protected override void OnCreate()
	{
		Type = WeaponType.EnergyCannon;
		Name = "energyball";
		Cooldown = 3.0f;
	}

	protected override void Fire()
	{
		Scene.CreateEntity(new EnergyBallPreset()
		{
			StartPosition = Transform.LocalPosition + Transform.Up * 10,
			Direction = Transform.Up
		}, Scene.Root);
	}
}