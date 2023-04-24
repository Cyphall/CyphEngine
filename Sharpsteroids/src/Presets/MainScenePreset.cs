using CyphEngine.Scenes;
using Sharpsteroids.Scripts;

namespace Sharpsteroids.Presets;

public class MainScenePreset : IScenePreset
{
	void IScenePreset.OnApply(Scene scene)
	{
		scene.CreateEntity(new CameraPreset(), scene.Root);
		
		scene.AllowCollisions("player", "asteroid");
		scene.AllowCollisions("bullet", "asteroid");

		scene.CreateMainScript<MainSceneScript>();
	}
}