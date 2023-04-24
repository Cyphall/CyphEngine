using JetBrains.Annotations;
using OpenTK.Mathematics;
using TKMathHelper = OpenTK.Mathematics.MathHelper;

namespace CyphEngine.Helper;

[PublicAPI]
public static class MathHelper
{
	private static Random _random = new Random(Guid.NewGuid().GetHashCode());
	
	public static float Modulo(float a, float b)
	{
		return (float)(a - b * Math.Floor(a / b));
	}

	public static Vector2 Rotate(this Vector2 vec, float angle)
	{
		float angleRad = TKMathHelper.DegreesToRadians(angle);
		
		return new Vector2
		{
			X = vec.X * MathF.Cos(angleRad) - vec.Y * MathF.Sin(angleRad),
			Y = vec.X * MathF.Sin(angleRad) + vec.Y * MathF.Cos(angleRad)
		};
	}

	public static Vector2 VectorFromAngle(float angle)
	{
		float angleRad = TKMathHelper.DegreesToRadians(angle);
		return new Vector2
		{
			X = MathF.Sin(angleRad),
			Y = MathF.Cos(angleRad)
		};
	}

	public static float AngleFromVector(Vector2 direction)
	{
		return TKMathHelper.RadiansToDegrees(MathF.Atan2(direction.X, direction.Y));
	}

	public static Vector2 RandomDirection()
	{
		return VectorFromAngle(_random.NextSingle() * 360);
	}

	public static float RandomFloat(float inclusiveMin = 0.0f, float inclusiveMax = 1.0f)
	{
		return TKMathHelper.Lerp(inclusiveMin, inclusiveMax, _random.NextSingle());
	}

	public static float RandomInt(int inclusiveMin, int inclusiveMax)
	{
		return _random.Next(inclusiveMin, inclusiveMax+1);
	}

	public static bool RandomBool()
	{
		return _random.Next(0, 2) == 1;
	}
	
	public static float Remap(float value, float fromMin, float fromMax, float toMin, float toMax)
	{
		return toMin + (value - fromMin) * (toMax - toMin) / (fromMax - fromMin);
	}
}
