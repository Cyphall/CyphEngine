using CyphEngine.Components;
using OpenTK.Mathematics;

namespace CyphEngine.Helper;

public static class PhysicsHelper
{
	public static bool Collides(ACollider object1, ACollider object2)
	{
		{
			if (object1 is BoxCollider box1 && object2 is BoxCollider box2)
			{
				return CollidesBoxBox(box1, box2);
			}
		}

		{
			if (object1 is CircleCollider circle1 && object2 is CircleCollider circle2)
			{
				return CollidesCircleCircle(circle1, circle2);
			}
		}

		{
			if (object1 is CircleCollider circle && object2 is BoxCollider box)
			{
				return CollidesBoxCircle(box, circle);
			}
		}

		{
			if (object1 is BoxCollider box && object2 is CircleCollider circle)
			{
				return CollidesBoxCircle(box, circle);
			}
		}

		Console.WriteLine($"Warning: unsupported collision check between {object1.GetType().Name} and {object2.GetType().Name}");
		return false;
	}

	#region CircleCircle
	
	private static bool CollidesCircleCircle(CircleCollider circle1, CircleCollider circle2)
	{
		float circleToCircleDistance = (circle1.LocalToWorld.ExtractTranslation() - circle2.LocalToWorld.ExtractTranslation()).Length;

		return circleToCircleDistance < circle1.Radius + circle2.Radius;
	}
	
	#endregion
	#region BoxBox
	
	private static bool CollidesBoxBox(BoxCollider box1, BoxCollider box2)
	{
		if (!CollidesBoxBoxFast(box1, box2))
		{
			return false;
		}
		
		if (!CollidesBoxBoxFull(box1, box2))
		{
			return false;
		}
		
		if (!CollidesBoxBoxFull(box2, box1))
		{
			return false;
		}

		return true;
	}

	private static bool CollidesBoxBoxFast(BoxCollider box1, BoxCollider box2)
	{
		Vector2 box1Corner = box1.Size / 2;
		float box1Radius = (box1Corner * box1.LocalToWorld.ExtractScale().Xy).LengthFast + 0.01f;
		
		Vector2 box2Corner = box2.Size / 2;
		float box2Radius = (box2Corner * box2.LocalToWorld.ExtractScale().Xy).LengthFast + 0.01f;

		float box1ToBox2Distance = (box1.LocalToWorld.ExtractTranslation() - box2.LocalToWorld.ExtractTranslation()).LengthFast + 0.01f;

		return box1ToBox2Distance < box1Radius + box2Radius;
	}

	private static bool CollidesBoxBoxFull(BoxCollider box1, BoxCollider box2)
	{
		(float box1HalfWidth, float box1HalfHeight) = box1.Size / 2;
		(float box2HalfWidth, float box2HalfHeight) = box2.Size / 2;

		// Get local corners of box1
		Vector2[] box1CornersInBox2Space = {
			new Vector2(-box1HalfWidth, +box1HalfHeight), // top left
			new Vector2(+box1HalfWidth, +box1HalfHeight), // top right
			new Vector2(+box1HalfWidth, -box1HalfHeight), // bottom right
			new Vector2(-box1HalfWidth, -box1HalfHeight)  // bottom left
		};
		
		// Transform the box1 corners in box2 space
		Matrix4 box1ToBox2 = box1.LocalToWorld * box2.WorldToLocal;
		for (int i = 0; i < box1CornersInBox2Space.Length; i++)
		{
			(float x, float y) = box1CornersInBox2Space[i];
			Vector4 pos = new Vector4(x, y, 0, 1);
			pos = pos * box1ToBox2;
			box1CornersInBox2Space[i] = pos.Xy;
		}

		// X axis
		if (AreAllPointsSeparated(box1CornersInBox2Space, 0, box2HalfWidth))
		{
			return false;
		}

		// Y axis
		if (AreAllPointsSeparated(box1CornersInBox2Space, 1, box2HalfHeight))
		{
			return false;
		}

		return true;
	}

	private static bool AreAllPointsSeparated(Vector2[] box1Points, int index, float box2HalfSize)
	{
		GetMinMax(box1Points, index, out float box1Min, out float box1Max);
		return box1Min > box2HalfSize || box1Max < -box2HalfSize;
	}

	private static void GetMinMax(Vector2[] values, int index, out float min, out float max)
	{
		min = values[0][index];
		max = values[0][index];

		for (int i = 1; i < values.Length; i++)
		{
			min = Math.Min(min, values[i][index]);
			max = Math.Max(max, values[i][index]);
		}
	}
	
	#endregion
	#region BoxCircle

	private static bool CollidesBoxCircle(BoxCollider box, CircleCollider circle)
	{
		if (!CollidesBoxCircleFast(box, circle))
		{
			return false;
		}

		return CollidesBoxCircleFull(box, circle);
	}
	
	private static bool CollidesBoxCircleFast(BoxCollider box, CircleCollider circle)
	{
		Vector2 boxCorner = box.Size / 2;
		float boxRadius = (boxCorner * box.LocalToWorld.ExtractScale().Xy).LengthFast + 0.01f;
		
		float circleRadius = circle.Radius + 0.01f;

		float box1ToBox2Distance = (box.LocalToWorld.ExtractTranslation() - circle.LocalToWorld.ExtractTranslation()).LengthFast + 0.01f;

		return box1ToBox2Distance < boxRadius + circleRadius;
	}
	
	private static bool CollidesBoxCircleFull(BoxCollider box, CircleCollider circle)
	{
		(float boxHalfWidth, float boxHalfHeight) = box.Size / 2;

		// Get local corners of box1
		Vector2 circleCenterInBoxSpace = (circle.LocalToWorld * box.WorldToLocal).ExtractTranslation().Xy;

		Vector2 closestPointOnBox = new Vector2
		{
			X = Math.Clamp(circleCenterInBoxSpace.X, -boxHalfWidth, boxHalfWidth),
			Y = Math.Clamp(circleCenterInBoxSpace.Y, -boxHalfHeight, boxHalfHeight)
		};

		return (circleCenterInBoxSpace - closestPointOnBox).Length < circle.Radius;
	}

	#endregion
}