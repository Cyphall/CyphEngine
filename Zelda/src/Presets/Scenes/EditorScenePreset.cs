using CyphEngine.Scenes;
using Zelda.Presets.UI;
using Zelda.Scripts;

namespace Zelda.Presets.Scenes;

public class EditorScenePreset : IScenePreset
{
	public void OnApply(Scene scene)
	{
		EditorMainScript editorMainScript = scene.CreateMainScript<EditorMainScript>();
		
		scene.UIManager.SetUI(new EditorUIPreset(editorMainScript));
	}
}