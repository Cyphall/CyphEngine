using CyphEngine.Rendering;
using JetBrains.Annotations;
using OpenTK.Mathematics;

namespace CyphEngine.Components;

[PublicAPI]
public class PhysicsCollider : AComponent
{
	private ACollider? _collider;

	public ulong LayerFlag { get; private set; }
	public string LayerName
	{
		set => LayerFlag = Scene.GetLayerFlag(value);
	}

	public ACollider? Collider
	{
		get
		{
			if (_collider == null)
				return null;
			
			_collider.WorldToLocal = Transform.WorldToLocalMatrix;
			_collider.LocalToWorld = Transform.LocalToWorldMatrix;

			return _collider;
		}
		set => _collider = value;
	}

	public bool DrawDebug { get; set; }
	public Vector4 DebugColor { get; set; } = new Vector4(0, 1, 0, 1);

	protected internal override void OnUpdate(float deltaTime)
	{
		if (Collider != null)
		{
			Collider.WorldToLocal = Transform.WorldToLocalMatrix;
			Collider.LocalToWorld = Transform.LocalToWorldMatrix;
		}
	}

	protected internal override void OnRender(Renderer renderer, ref Matrix4 viewProjection)
	{
		if (!DrawDebug)
			return;

		switch (Collider)
		{
			case null:
				return;
			case BoxCollider boxCollider:
				RenderBoxCollider(renderer, ref viewProjection, boxCollider);
				break;
			case CircleCollider circleCollider:
				RenderCircleCollider(renderer, ref viewProjection, circleCollider);
				break;
		}
	}

	private void RenderBoxCollider(Renderer renderer, ref Matrix4 viewProjection, BoxCollider collider)
	{
		Matrix4 localToView = Matrix4.CreateScale(collider.Size.X, collider.Size.Y, 1) * Transform.LocalToWorldMatrix * viewProjection;
		
		renderer.AddWireframeBoxRequest(DebugColor, localToView);
	}

	private void RenderCircleCollider(Renderer renderer, ref Matrix4 viewProjection, CircleCollider collider)
	{
		Matrix4 localToView = Matrix4.CreateScale(collider.Radius, collider.Radius, 1) * Transform.LocalToWorldMatrix * viewProjection;
		
		renderer.AddWireframeCircleRequest(DebugColor, localToView);
	}
}