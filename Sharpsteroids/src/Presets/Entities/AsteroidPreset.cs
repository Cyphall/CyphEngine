using CyphEngine.Components;
using CyphEngine.Entities;
using OpenTK.Mathematics;
using Sharpsteroids.Scripts;

namespace Sharpsteroids.Presets;

public class AsteroidPreset : IEntityPreset
{
	private Tier _tier;
	private Vector2 _position;
	private Vector2 _direction;

	public AsteroidPreset(Tier tier, Vector2 position, Vector2 direction)
	{
		_tier = tier;
		_position = position;
		_direction = direction;
	}

	public void OnApply(Entity entity)
	{
		AsteroidScript script = entity.CreateComponent<AsteroidScript>();
		script.Init(_tier, _position, _direction);

		string texturePath;
		float radius;
		switch (_tier)
		{
			case Tier.Big:
				texturePath = "assets/sprites/asteroid3.png";
				radius = 40;
				break;
			case Tier.Medium:
				texturePath = "assets/sprites/asteroid2.png";
				radius = 27;
				break;
			case Tier.Small:
				texturePath = "assets/sprites/asteroid1.png";
				radius = 13;
				break;
			default:
				throw new InvalidOperationException();
		}

		SpriteRenderer spriteRenderer = entity.CreateComponent<SpriteRenderer>();
		spriteRenderer.LoadTexture(texturePath, true, true);

		PhysicsCollider collider = entity.CreateComponent<PhysicsCollider>();
		collider.Collider = new CircleCollider(radius);
		collider.LayerName = "asteroid";
	}
}