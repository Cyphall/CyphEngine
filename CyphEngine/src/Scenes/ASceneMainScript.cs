using CyphEngine.Rendering;
using JetBrains.Annotations;

namespace CyphEngine.Scenes;

[PublicAPI]
public class ASceneMainScript
{
	public Scene Scene { get; internal set; } = null!;
	public Engine Engine => Scene.Engine;
	public Window Window => Engine.Window;
	
	protected internal virtual void OnUpdate(float deltaTime) {}
	protected internal virtual void OnRender(Renderer renderer) {}
}