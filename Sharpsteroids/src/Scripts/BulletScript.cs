using CyphEngine.Components;
using OpenTK.Mathematics;

namespace Sharpsteroids.Scripts;

public class BulletScript : AComponent
{
	private PhysicsCollider _collider = null!;
	
	public Vector2 Direction
	{
		get => Transform.Up;
		set => Transform.LocalRotation = MathHelper.RadiansToDegrees(MathF.Atan2(value.X, value.Y));
	}

	public float Speed { get; set; }
	public bool DestroyedOnImpact { get; set; }

	private bool _destroyed;

	protected override void OnInit()
	{
		_collider = Entity.GetComponent<PhysicsCollider>()!;
	}

	protected override void OnUpdate(float deltaTime)
	{
		Transform.LocalPosition += Direction * Speed * deltaTime;

		if (!Scene.MainCamera!.IsInView(_collider.Collider!))
		{
			Scene.DestroyEntity(Entity);
		}
	}

	protected override void OnCollide(PhysicsCollider thisPhysicsCollider, PhysicsCollider otherPhysicsCollider)
	{
		if (!_destroyed)
		{
			otherPhysicsCollider.Entity.GetComponent<AsteroidScript>()!.OnBulletHit();

			if (DestroyedOnImpact)
			{
				Scene.DestroyEntity(Entity);
				_destroyed = true;
			}
		}
	}
}