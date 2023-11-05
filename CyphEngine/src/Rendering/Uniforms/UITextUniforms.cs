using System.Runtime.InteropServices;
using OpenTK.Mathematics;

namespace CyphEngine.Rendering.Uniforms;

[StructLayout(LayoutKind.Explicit)]
public struct UITextUniforms
{
	[FieldOffset(0)] public ulong Texture;
	[FieldOffset(8)] public float SDFAlpha0Value;
	[FieldOffset(12)] public float SDFAlpha1Value;
	[FieldOffset(16)] public Matrix4 Matrix;
	[FieldOffset(80)] public Vector4 ColorMask;
	[FieldOffset(96)] public Vector2 MinUV;
	[FieldOffset(104)] public Vector2 MaxUV;
}