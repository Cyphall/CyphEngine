using CyphEngine.Helper;
using JetBrains.Annotations;
using OpenTK.Mathematics;

namespace CyphEngine.Components;

[PublicAPI]
public class Camera : AComponent
{
	public Matrix4 Projection { get; private set; }
	public Matrix4 View => Transform.WorldToLocalMatrix;
	public Vector2 CursorWorldPos
	{
		get
		{
			Vector2 cursorPosNormalized = Window.CursorPos / Window.SimulatedSize;

			Vector2 cursorPos11 = cursorPosNormalized * new Vector2(2.0f) - new Vector2(1.0f);
			cursorPos11.Y = -cursorPos11.Y;

			Vector4 ndc = new Vector4(cursorPos11.X, cursorPos11.Y, 0, 1);

			return ((View * Projection).Inverted() * ndc).Xy;
		}
	}

	protected internal override void OnCreate()
	{
		(float width, float height) = Window.SimulatedSize;

		Projection = Matrix4.CreateOrthographic(width, height, -1000, 1000);

		if (Scene.MainCamera == null && Scene.HasScheduledCameraChange == false)
		{
			Scene.ScheduleCameraChange(this);
		}
	}

	public bool IsInView(Vector2 point)
	{
		Matrix4 vp = View * Projection;

		(float x, float y) = point;
		Vector4 transformedEntityPos = vp * new Vector4(x, y, 0, 1);
		transformedEntityPos /= transformedEntityPos.W;

		return transformedEntityPos.X is <= 1 and >= -1 &&
		       transformedEntityPos.Y is <= 1 and >= -1;
	}

	public bool IsInView(ACollider collider)
	{
		BoxCollider viewCollider = new BoxCollider(Window.SimulatedSize);
		viewCollider.WorldToLocal = View;
		viewCollider.LocalToWorld = Matrix4.Invert(viewCollider.WorldToLocal);

		return PhysicsHelper.Collides(viewCollider, collider);
	}
}