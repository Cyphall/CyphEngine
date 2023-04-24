using CyphEngine.Helper;
using OpenTK.Mathematics;
using Sharpsteroids.Presets.Entities.Bullets;

namespace Sharpsteroids.Scripts.Weapons;

public class ShotgunScript : AWeaponScript
{
	protected override void OnCreate()
	{
		Type = WeaponType.Shotgun;
		Name = "shotgun";
		Cooldown = 1.5f;
	}

	protected override void Fire()
	{
		Vector2 direction = Transform.Up;

		for (int i = 0; i < 5; i++)
		{
			Scene.CreateEntity(new ShotgunBulletPreset
			{
				StartPosition = Transform.LocalPosition + Transform.Up * 10,
				Direction = direction.Rotate(-10 + i * 5)
			}, Scene.Root);
		}
	}
}