using JetBrains.Annotations;

namespace CyphEngine;

[PublicAPI]
public enum StartPosition
{
	DontCare,
	Center
}

[PublicAPI]
public sealed class WindowSettings
{
	public uint Width { get; set; } = 800;
	public uint Height { get; set; } = 600;
	public string Title { get; set; } = "CyphEngine Window";
	public StartPosition StartPosition { get; set; } = StartPosition.DontCare;
}