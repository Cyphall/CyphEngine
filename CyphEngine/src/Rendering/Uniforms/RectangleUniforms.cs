using System.Runtime.InteropServices;
using OpenTK.Mathematics;

namespace CyphEngine.Rendering.Uniforms;

[StructLayout(LayoutKind.Sequential)]
public struct RectangleUniforms
{
	public Vector4 FillColor;
	public Vector4 BorderColor;
	public Matrix4 Matrix;
	public Vector2 RectangleSize;
	public float CornerRadius;
	public float DpiScaling;
	private Vector3 _padding;
	public float BorderThickness;
}