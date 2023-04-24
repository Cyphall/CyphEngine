using CyphEngine.Helper;
using CyphEngine.UI;
using OpenTK.Mathematics;
using Zelda.Presets.Scenes;

namespace Zelda.Presets.UI;

public class MainMenuUIPreset : IUIPreset
{
	public void OnApply(UIRoot root)
	{
		UIStackPanel stackPanel = new UIStackPanel();
		stackPanel.VerticalAlignment = VerticalAlignment.Center;
		stackPanel.HorizontalAlignment = HorizontalAlignment.Center;
		root.Child = stackPanel;

		(UIButton playButton, UIText _) = UIHelper.AddTextButton(new Vector4(1, 1, 1 ,1), new Vector4(0.2f, 0.2f, 0.2f, 1), new Vector4(0, 0, 0, 0), 10, 0, "Play", "assets/fonts/Roboto-Medium.ttf", 30);
		playButton.Margin = 20;
		// playButton.Click += _ => root.Engine.ScheduleSceneLoad();
		stackPanel.AddChild(playButton);
		
		(UIButton editorButton, UIText _) = UIHelper.AddTextButton(new Vector4(1, 1, 1 ,1), new Vector4(0.2f, 0.2f, 0.2f, 1), new Vector4(0, 0, 0, 0), 10, 0, "Editor", "assets/fonts/Roboto-Medium.ttf", 30);
		editorButton.Margin = 20;
		editorButton.Click += _ => root.Engine.ScheduleSceneLoad(new EditorScenePreset());
		stackPanel.AddChild(editorButton);

		(UIButton quitButton, UIText _) = UIHelper.AddTextButton(new Vector4(1, 1, 1 ,1), new Vector4(0.2f, 0.2f, 0.2f, 1), new Vector4(0, 0, 0, 0), 10, 0, "Quit", "assets/fonts/Roboto-Medium.ttf", 30);
		quitButton.Margin = 20;
		quitButton.Click += _ => root.Engine.Quit();
		stackPanel.AddChild(quitButton);
	}
}