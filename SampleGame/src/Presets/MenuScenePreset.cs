using CyphEngine.Scenes;

namespace SampleGame;

public class MenuScenePreset : IScenePreset
{
	void IScenePreset.OnApply(Scene scene)
	{
		scene.UIManager.SetUI(new MenuUIPreset());
	}
}