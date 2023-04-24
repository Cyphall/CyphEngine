using CyphEngine.Helper;
using CyphEngine.UI;
using OpenTK.Mathematics;
using Zelda.Presets.Scenes;
using Zelda.Scripts;

namespace Zelda.Presets.UI;

public class EditorUIPreset : IUIPreset
{
	private EditorMainScript _mainScript;

	public EditorUIPreset(EditorMainScript mainScript)
	{
		_mainScript = mainScript;
	}

	private static List<Vector2i> GetValidTiles()
	{
		List<Vector2i> validTiles = new List<Vector2i>();

		for (int y = 0; y < 13; y++)
		{
			for (int x = 0; x < 24; x++)
			{
				validTiles.Add(new Vector2i(x, y));
			}
		}

		for (int y = 13; y < 16; y++)
		{
			for (int x = 0; x < 13; x++)
			{
				validTiles.Add(new Vector2i(x, y));
			}
		}

		for (int y = 13; y < 25; y++)
		{
			for (int x = 16; x < 24; x++)
			{
				validTiles.Add(new Vector2i(x, y));
			}
		}

		return validTiles;
	}

	public void OnApply(UIRoot root)
	{
		UIDockPanel mainDockPanel = new UIDockPanel();
		root.Child = mainDockPanel;
		
		UIRectangle background = new UIRectangle();
		background.FillColor = new Vector4(0.521f, 0.756f, 0.913f, 1);
		background.CornerRadius = 10;
		background.Width = 200;
		mainDockPanel.AddChild(background, DockSide.Right);

		UIDockPanel menuDockPanel = new UIDockPanel();
		background.Child = menuDockPanel;

		UIStackPanel buttonsPanel = new UIStackPanel();
		buttonsPanel.Direction = Direction.RightToLeft;
		menuDockPanel.AddChild(buttonsPanel, DockSide.Bottom);
		
		(UIButton backButton, UIText _) = UIHelper.AddTextButton(new Vector4(1, 1, 1, 1), new Vector4(0.2f, 0.2f, 0.2f, 1), new Vector4(0, 0, 0, 0), 3, 0, "Back", "assets/fonts/Roboto-Medium.ttf", 20);
		backButton.Margin = 5;
		backButton.Click += button => button.Engine.ScheduleSceneLoad(new MainMenuScenePreset());
		buttonsPanel.AddChild(backButton);
		
		(UIButton saveButton, UIText _) = UIHelper.AddTextButton(new Vector4(1, 1, 1, 1), new Vector4(0.2f, 0.2f, 0.2f, 1), new Vector4(0, 0, 0, 0), 3, 0, "Save", "assets/fonts/Roboto-Medium.ttf", 20);
		saveButton.Margin = 5;
		buttonsPanel.AddChild(saveButton);

		UIRectangle listRectangle = new UIRectangle();
		listRectangle.FillColor = new Vector4(1, 1, 1 ,1);
		menuDockPanel.AddChild(listRectangle);
		
		List<Vector2i> validTiles = GetValidTiles();

		for (int i = 0; i < 5; i++)
		{
			
		}
	}
}