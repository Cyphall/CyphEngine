using JetBrains.Annotations;
using OpenTK.Mathematics;

namespace CyphEngine.Components;

[PublicAPI]
public class BoxCollider : ACollider
{
	public Vector2 Size { get; set; }

	public BoxCollider(Vector2 size)
	{
		Size = size;
	}
}