using JetBrains.Annotations;
using OpenTK.Mathematics;

namespace CyphEngine.Components;

[PublicAPI]
public abstract class ACollider
{
	public Matrix4 LocalToWorld { get; set; }
	public Matrix4 WorldToLocal { get; set; }
}