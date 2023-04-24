using CyphEngine.Components;
using CyphEngine.Entities;
using OpenTK.Mathematics;

namespace Sharpsteroids.Scripts;

public class SniperBulletScript : AComponent
{
	private Entity _colliderEntity = null!;
	private PhysicsCollider _collider = null!;
	private Vector2 _halfBulletSize;
	private Vector2 _bulletSize;
	
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
		_bulletSize = Entity.GetComponent<SpriteRenderer>()!.Size;
		_halfBulletSize = _bulletSize / 2;
		
		_colliderEntity = Scene.CreateEntity(IEntityPreset.Empty, Transform, "sniper bullet hitbox");
		
		_collider = _colliderEntity.CreateComponent<PhysicsCollider>();
		_collider.Collider = new BoxCollider(_bulletSize);
		_collider.LayerName = "bullet";

		SniperHitboxScript sniperHitboxScript = _colliderEntity.CreateComponent<SniperHitboxScript>();
		sniperHitboxScript.OwnerScript = this;
	}

	protected override void OnUpdate(float deltaTime)
	{
		float deltaPos = Speed * deltaTime;
		Transform.LocalPosition += Direction * deltaPos;

		Vector2 neededSize = _bulletSize + new Vector2(0, deltaPos);
		
		((BoxCollider)_collider.Collider!).Size = neededSize;
		_colliderEntity.Transform.LocalPosition = new Vector2(0, deltaPos) / 2;

		if (!Scene.MainCamera!.IsInView(_collider.Collider!))
		{
			Scene.DestroyEntity(Entity);
		}
	}

	public void Collide(PhysicsCollider thisPhysicsCollider, PhysicsCollider otherPhysicsCollider)
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