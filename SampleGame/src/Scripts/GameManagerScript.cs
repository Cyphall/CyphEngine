using CyphEngine.Components;
using CyphEngine.UI;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace SampleGame;

public class GameManagerScript : AComponent
{
	public void Pause()
	{
		if (!Scene.TimePaused)
		{
			Scene.PauseTime();
			Scene.UIManager.SetUI(new PauseUIPreset());
		}
	}

	public void Unpause()
	{
		if (Scene.TimePaused)
		{
			Scene.UnpauseTime();
			Scene.UIManager.SetUI(IUIPreset.Empty);
		}
	}
	
	protected override void OnUpdate(float deltaTime)
	{
		if (Window.KeyPressed(Keys.Escape))
		{
			Pause();
		}
	}
}