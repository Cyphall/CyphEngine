using CyphEngine.Helper;
using CyphEngine.UI;
using OpenTK.Mathematics;

namespace SampleGame;

public class PauseUIPreset : IUIPreset
{
	public void OnApply(UIRoot root)
	{
		UIGrid grid = new UIGrid();
		root.Child = grid;
		
		UIImage background = new UIImage();
		background.SetTexture(new Vector4(0, 0, 0, 0.5f));
		grid.AddChild(background);

		UIStackPanel stackPanel = new UIStackPanel();
		stackPanel.VerticalAlignment = VerticalAlignment.Center;
		stackPanel.HorizontalAlignment = HorizontalAlignment.Center;
		grid.AddChild(stackPanel);
		
		(UIButton resumeButton, UIText _) = UIHelper.AddTextButton(new Vector4(1, 1, 1 ,1), new Vector4(0.2f, 0.2f, 0.2f, 1), new Vector4(0, 0, 0, 0), 10, 0, "Resume", "assets/fonts/Retro Gaming.ttf", 40);
		resumeButton.Margin = 20;
		resumeButton.Click += _ => (root.Scene.MainScript as GameMainScript)!.GameManagerScript.Unpause();
		stackPanel.AddChild(resumeButton);

		(UIButton menuButton, UIText _) = UIHelper.AddTextButton(new Vector4(1, 1, 1 ,1), new Vector4(0.2f, 0.2f, 0.2f, 1), new Vector4(0, 0, 0, 0), 10, 0, "Menu", "assets/fonts/Retro Gaming.ttf", 40);
		menuButton.Margin = 20;
		menuButton.Click += _ => root.Engine.ScheduleSceneLoad(new MenuScenePreset());
		stackPanel.AddChild(menuButton);
	}
}