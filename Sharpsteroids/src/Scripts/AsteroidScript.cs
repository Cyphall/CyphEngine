using CyphEngine.Components;
using CyphEngine.Entities;
using OpenTK.Mathematics;
using Sharpsteroids.Presets;
using MathHelper = CyphEngine.Helper.MathHelper;
using TKMathHelper = OpenTK.Mathematics.MathHelper;

namespace Sharpsteroids.Scripts;

public enum Tier
{
	Big,
	Medium,
	Small
}

public class AsteroidScript : AComponent
{
	private PhysicsCollider _collider = null!;

	private Tier _tier;
	private Vector2 _velocity = MathHelper.RandomDirection() * MathHelper.RandomFloat(10.0f, 100.0f);
	private float _angularVelocity = MathHelper.RandomFloat(-200.0f, 200.0f);

	public void Init(Tier tier, Vector2 position, Vector2 direction)
	{
		Transform.LocalPosition = position;
		_tier = tier;
		
		float velocityWeight = MathHelper.RandomFloat();
		float angularVelocityWeight = 1 - velocityWeight;
						
		_velocity = direction * TKMathHelper.Lerp(10.0f, 100.0f, velocityWeight);

		int sign = MathHelper.RandomBool() ? 1 : -1;
		_angularVelocity = TKMathHelper.Lerp(0.0f, 200.0f, angularVelocityWeight) * sign;
	}
	
	protected override void OnInit()
	{
		_collider = Entity.GetComponent<PhysicsCollider>()!;
	}

	private void CreateSmallerAsteroids(Tier tier)
	{
		Vector2 randomDirection = MathHelper.RandomDirection();

		for (int i = 0; i < 2; i++)
		{
			Entity asteroid = Scene.CreateEntity(new AsteroidPreset(tier, Transform.LocalPosition, randomDirection), Scene.Root);
			((MainSceneScript)Scene.MainScript!).Asteroids.Add(asteroid);
			
			randomDirection = -randomDirection;
		}
	}

	protected override void OnDestroy()
	{
		((MainSceneScript)Scene.MainScript!).Asteroids.Remove(Entity);
		
		int debrisCount;
		string explosionSound;
		switch (_tier)
		{
			case Tier.Big:
				CreateSmallerAsteroids(Tier.Medium);
				debrisCount = 18;
				explosionSound = "assets/sounds/explosion-large.wav";
				break;
			case Tier.Medium:
				CreateSmallerAsteroids(Tier.Small);
				debrisCount = 10;
				explosionSound = "assets/sounds/explosion-medium.wav";
				break;
			case Tier.Small:
				debrisCount = 4;
				explosionSound = "assets/sounds/explosion-small.wav";
				break;
			default:
				throw new InvalidOperationException();
		}
		
		Scene.CreateEntity(new ExplosionPreset(explosionSound, Transform.LocalPosition, _velocity, debrisCount, 1), Scene.Root);
	}

	public void OnBulletHit()
	{
		Scene.DestroyEntity(Entity);
	}

	protected override void OnUpdate(float deltaTime)
	{
		Transform.LocalPosition += _velocity * deltaTime;
		Transform.LocalRotation += _angularVelocity * deltaTime;

		float radius = ((CircleCollider)_collider.Collider!).Radius;
		Vector2 offsettedSceneHalfSize = Window.SimulatedSize / 2.0f + new Vector2(radius);

		Vector2 currentPosition = Transform.LocalPosition;
		Vector2 newPostion = currentPosition;

		if (currentPosition.X < -offsettedSceneHalfSize.X)
		{
			newPostion.X += offsettedSceneHalfSize.X * 2;
		}
		else if (currentPosition.X > offsettedSceneHalfSize.X)
		{
			newPostion.X -= offsettedSceneHalfSize.X * 2;
		}
		else if (currentPosition.Y < -offsettedSceneHalfSize.Y)
		{
			newPostion.Y += offsettedSceneHalfSize.Y * 2;
		}
		else if (currentPosition.Y > offsettedSceneHalfSize.Y)
		{
			newPostion.Y -= offsettedSceneHalfSize.Y * 2;
		}

		if (newPostion != currentPosition)
		{
			Transform.LocalPosition = newPostion;
		}
	}
}