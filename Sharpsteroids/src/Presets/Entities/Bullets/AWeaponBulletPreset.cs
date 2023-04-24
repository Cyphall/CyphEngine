using CyphEngine.Entities;
using OpenTK.Mathematics;

namespace Sharpsteroids.Presets.Weapons;

public abstract class AWeaponBulletPreset : IEntityPreset
{
	public Vector2 StartPosition { get; set; }
	public Vector2 Direction { get; set; }
	public abstract void OnApply(Entity entity);
}