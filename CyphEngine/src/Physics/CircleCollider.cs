using JetBrains.Annotations;

namespace CyphEngine.Components;

[PublicAPI]
public class CircleCollider : ACollider
{
	public float Radius { get; set; }

	public CircleCollider(float radius)
	{
		Radius = radius;
	}
}