using CyphEngine.Helper;
using CyphEngine.UI;
using OpenTK.Mathematics;

namespace SampleGame;

public class MenuUIPreset : IUIPreset
{
	public void OnApply(UIRoot root)
	{
		UIStackPanel stackPanel = new UIStackPanel();
		stackPanel.VerticalAlignment = VerticalAlignment.Center;
		stackPanel.HorizontalAlignment = HorizontalAlignment.Center;
		root.Child = stackPanel;

		(UIButton playButton, UIText _) = UIHelper.AddTextButton(new Vector4(1, 1, 1 ,1), new Vector4(0.2f, 0.2f, 0.2f, 1), new Vector4(0, 0, 0, 0), 10, 0, "Play", "assets/fonts/Retro Gaming.ttf", 40);
		playButton.Margin = 20;
		playButton.Click += _ => root.Engine.ScheduleSceneLoad(new MainScenePreset());
		stackPanel.AddChild(playButton);

		(UIButton quitButton, UIText _) = UIHelper.AddTextButton(new Vector4(1, 1, 1 ,1), new Vector4(0.2f, 0.2f, 0.2f, 1), new Vector4(0, 0, 0, 0), 10, 0, "Quit", "assets/fonts/Retro Gaming.ttf", 40);
		quitButton.Margin = 20;
		quitButton.Click += _ => root.Engine.Quit();
		stackPanel.AddChild(quitButton);
	}
}