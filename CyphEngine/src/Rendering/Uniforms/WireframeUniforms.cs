using System.Runtime.InteropServices;
using OpenTK.Mathematics;

namespace CyphEngine.Rendering.Uniforms;

[StructLayout(LayoutKind.Explicit)]
public struct WireframeUniforms
{
	[FieldOffset(0)] public Vector4 Color;
	[FieldOffset(16)] public Matrix4 Matrix;
}