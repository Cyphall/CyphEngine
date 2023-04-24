using CyphEngine.Scenes;
using CyphEngine.UI;
using OpenTK.Mathematics;

namespace Zelda.Scripts;

public class EditorMainScript : ASceneMainScript
{
	public Dictionary<Vector2i, UIImage> Tiles { get; } = new Dictionary<Vector2i, UIImage>();
}