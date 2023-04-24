using CyphEngine.Components;
using OpenTK.Mathematics;
using MathHelper = CyphEngine.Helper.MathHelper;

namespace Sharpsteroids.Scripts;

public class DebrisScript : AComponent
{
	public float Lifetime { get; set; }
	private Vector2 _velocity = MathHelper.RandomDirection() * MathHelper.RandomFloat(30, 100);

	public void AddVelocity(Vector2 velocity)
	{
		_velocity += velocity;
	}

	protected override void OnUpdate(float deltaTime)
	{
		Lifetime -= deltaTime;
		if (Lifetime < 0)
		{
			Scene.DestroyEntity(Entity);
		}

		Transform.LocalPosition += _velocity * deltaTime;
		_velocity *= 1 - 0.9f * deltaTime;
	}
}