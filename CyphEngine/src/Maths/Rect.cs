using JetBrains.Annotations;
using OpenTK.Mathematics;

namespace CyphEngine.Maths;

[PublicAPI]
public struct Rect
{
	public Vector2 Min;
	public Vector2 Max;
	
	public Vector2 Size
	{
		get => Max - Min;
		set => Max = value + Min;
	}

	public Vector2 TopLeft => Min;
	public Vector2 TopRight => new Vector2(Max.X, Min.Y);
	public Vector2 BottomLeft => new Vector2(Min.X, Max.Y);
	public Vector2 BottomRight => Max;

	public bool Contains(Vector2 point)
	{
		return point.X >= Min.X && point.X <= Max.X && point.Y >= Min.Y && point.Y <= Max.Y;
	}

	public static Rect FromTwoPoints(Vector2 min, Vector2 max)
	{
		return new Rect
		{
			Min = min,
			Max = max
		};
	}

	public static Rect FromOriginSize(Vector2 origin, Vector2 size)
	{
		return new Rect
		{
			Min = origin,
			Max = origin + size
		};
	}
}