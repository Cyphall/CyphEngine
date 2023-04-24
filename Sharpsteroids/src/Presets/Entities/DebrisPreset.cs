using CyphEngine.Components;
using CyphEngine.Entities;
using OpenTK.Mathematics;
using Sharpsteroids.Scripts;

namespace Sharpsteroids.Presets;

public class DebrisPreset : IEntityPreset
{
	private float _lifetime;
	private Vector2 _position;
	private Vector2 _initialObjectVelocity;

	public DebrisPreset(float lifetime, Vector2 position, Vector2 initialObjectVelocity)
	{
		_lifetime = lifetime;
		_position = position;
		_initialObjectVelocity = initialObjectVelocity;
	}
	
	public void OnApply(Entity entity)
	{
		DebrisScript script = entity.CreateComponent<DebrisScript>();
		script.Lifetime = _lifetime;
		script.AddVelocity(_initialObjectVelocity);

		entity.Transform.LocalPosition = _position;

		SpriteRenderer spriteRenderer = entity.CreateComponent<SpriteRenderer>();
		spriteRenderer.SetTexture(new Vector4(1, 1, 1, 1));
		spriteRenderer.Size = new Vector2(1);
	}
}