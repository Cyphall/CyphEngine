using CyphEngine.Scenes;

namespace TestGame;

public class TestScenePreset : IScenePreset
{
	void IScenePreset.OnApply(Scene scene)
	{
		scene.CreateEntity(new CameraEntityPreset(), scene.Root);
		// scene.CreateEntity(new TestEntityPreset(), scene.Root);

		scene.CreateMainScript<MainSceneScript>();

		scene.UIManager.SetUI(new TestUIPreset());
	}
}