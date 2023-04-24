using CyphEngine.Components;

namespace Sharpsteroids.Scripts;

public class SniperHitboxScript : AComponent
{
	public SniperBulletScript OwnerScript { get; set; } = null!;

	protected override void OnCollide(PhysicsCollider thisPhysicsCollider, PhysicsCollider otherPhysicsCollider)
	{
		OwnerScript.Collide(thisPhysicsCollider, otherPhysicsCollider);
	}
}