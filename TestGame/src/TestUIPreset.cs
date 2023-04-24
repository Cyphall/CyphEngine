using CyphEngine.Helper;
using CyphEngine.UI;
using OpenTK.Mathematics;

namespace TestGame;

public class TestUIPreset : IUIPreset
{
	public void OnApply(UIRoot root)
	{
		UIRectangle background = new UIRectangle();
		background.FillColor = new Vector4(0.941f, 0.941f, 0.941f, 1);
		root.Child = background;

		UIDockPanel dockPanel = new UIDockPanel();
		background.Child = dockPanel;
		
		(UIButton buttonTest, UIText _) = UIHelper.AddTextButton(new Vector4(0.980f, 0.980f, 0.980f, 1), new Vector4(0, 0, 0, 1), new Vector4(0.788f, 0.788f, 0.788f, 1), 5, 1, "The quick brown fox jumps over the lazy dog.", "C:/Windows/Fonts/calibri.ttf", 14);
		buttonTest.Height = 24;
		buttonTest.Margin = 10;
		dockPanel.AddChild(buttonTest);

		UIStackPanel stackPanel = new UIStackPanel();
		stackPanel.Direction = Direction.RightToLeft;
		dockPanel.AddChild(stackPanel, DockSide.Bottom);
		
		(UIButton buttonCancel, UIText _) = UIHelper.AddTextButton(new Vector4(0.980f, 0.980f, 0.980f, 1), new Vector4(0, 0, 0, 1), new Vector4(0.788f, 0.788f, 0.788f, 1), 5, 1, "Cancel", "C:/Windows/Fonts/calibri.ttf", 14);
		buttonCancel.Width = 86;
		buttonCancel.Height = 24;
		buttonCancel.Margin = 10;
		stackPanel.AddChild(buttonCancel);
		
		(UIButton buttonOk, UIText _) = UIHelper.AddTextButton(new Vector4(0.980f, 0.980f, 0.980f, 1), new Vector4(0, 0, 0, 1), new Vector4(0.788f, 0.788f, 0.788f, 1), 5, 1, "OK", "C:/Windows/Fonts/calibri.ttf", 14);
		buttonOk.Width = 86;
		buttonOk.Height = 24;
		buttonOk.Margin = 10;
		stackPanel.AddChild(buttonOk);
	}
}