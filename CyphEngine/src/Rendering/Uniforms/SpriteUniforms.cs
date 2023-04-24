using System.Runtime.InteropServices;
using OpenTK.Mathematics;

namespace CyphEngine.Rendering.Uniforms;

[StructLayout(LayoutKind.Explicit)]
public struct SpriteUniforms
{
	[FieldOffset(0)] public ulong Texture;
	[FieldOffset(16)] public Matrix4 Matrix;
	[FieldOffset(80)] public Vector4 ColorMask;
	[FieldOffset(96)] public Vector2 MinUV;
	[FieldOffset(104)] public Vector2 MaxUV;
}