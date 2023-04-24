using CyphEngine.Scenes;
using Zelda.Presets.UI;

namespace Zelda.Presets.Scenes;

public class MainMenuScenePreset : IScenePreset
{
	public void OnApply(Scene scene)
	{
		scene.UIManager.SetUI(new MainMenuUIPreset());
	}
}