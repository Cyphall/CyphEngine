using CyphEngine.Components;
using OpenTK.Mathematics;

namespace SampleGame;

public class BulletScript : AComponent
{
	private bool _destroyed;
	
	protected override void OnUpdate(float deltaTime)
	{
		Transform.LocalPosition += new Vector2(0, 720 * deltaTime);

		if (!Scene.MainCamera!.IsInView(Entity.Transform.WorldPosition))
		{
			Scene.DestroyEntity(Entity);
		}
	}

	protected override void OnCollide(PhysicsCollider thisCollider, PhysicsCollider otherCollider)
	{
		if (!_destroyed)
		{
			Scene.DestroyEntity(otherCollider.Entity);
			Scene.DestroyEntity(Entity);
			_destroyed = true;
		}
	}

	protected override void OnDestroy()
	{
		(Scene.MainScript as GameMainScript)!.PlayerBullet = null;
	}
}