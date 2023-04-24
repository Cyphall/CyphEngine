using CyphEngine.UI;
using OpenTK.Mathematics;

namespace CyphEngine.Helper;

public static class UIHelper
{
	public static (UIButton, UIImage) AddImageButton(string path)
	{
		UIButton button = new UIButton();

		UIGrid grid = new UIGrid();
		button.Child = grid;
		
		UIImage image = new UIImage();
		image.LoadTexture(path);
		grid.AddChild(image);
		
		UIRectangle hover = new UIRectangle();
		hover.FillColor = new Vector4(0, 0, 0, 0);
		grid.AddChild(hover);

		button.StateChange += sender => {
			hover.FillColor = sender.State switch
			{
				ButtonState.Normal => new Vector4(0, 0, 0, 0),
				ButtonState.Hovered => new Vector4(0, 0, 0, 0.2f),
				ButtonState.Pressed => new Vector4(0, 0, 0, 0.3f),
				_ => hover.FillColor
			};
		};
	
		return (button, image);
	}
	
	public static (UIButton, UIText) AddTextButton(Vector4 backgroundColor, Vector4 foregroundColor, Vector4 borderColor, float cornerRadius, float borderThickness, string text, string fontFilePath, float fontSize)
	{
		UIButton button = new UIButton();

		UIGrid grid = new UIGrid();
		button.Child = grid;
		
		UIRectangle rectangle = new UIRectangle();
		rectangle.FillColor = backgroundColor;
		rectangle.BorderColor = borderColor;
		rectangle.CornerRadius = cornerRadius;
		rectangle.BorderThickness = borderThickness;
		grid.AddChild(rectangle);
		
		UIText textElement = new UIText();
		textElement.Text = text;
		textElement.FontFile = fontFilePath;
		textElement.FontSize = fontSize;
		textElement.HorizontalAlignment = HorizontalAlignment.Center;
		textElement.VerticalAlignment = VerticalAlignment.Center;
		textElement.ColorMask = foregroundColor;
		textElement.Margin = 2;
		rectangle.Child = textElement;
		
		UIRectangle hover = new UIRectangle();
		hover.FillColor = new Vector4(0, 0, 0, 0);
		hover.CornerRadius = cornerRadius;
		grid.AddChild(hover);

		button.StateChange += sender => {
			hover.FillColor = sender.State switch
			{
				ButtonState.Normal => new Vector4(0, 0, 0, 0),
				ButtonState.Hovered => new Vector4(0, 0, 0, 0.2f),
				ButtonState.Pressed => new Vector4(0, 0, 0, 0.3f),
				_ => hover.FillColor
			};
		};
	
		return (button, textElement);
	}
	
	public static (UIButton, UIImage, UIText) AddImageTextButton(string path, Vector4 foregroundColor, string text, string fontFilePath, float fontSize)
	{
		UIButton button = new UIButton();

		UIGrid grid = new UIGrid();
		button.Child = grid;
		
		UIImage image = new UIImage();
		image.LoadTexture(path);
		grid.AddChild(image);
		
		UIText textElement = new UIText();
		textElement.Text = text;
		textElement.FontFile = fontFilePath;
		textElement.FontSize = fontSize;
		textElement.HorizontalAlignment = HorizontalAlignment.Center;
		textElement.VerticalAlignment = VerticalAlignment.Center;
		textElement.ColorMask = foregroundColor;
		grid.AddChild(textElement);
		
		UIRectangle hover = new UIRectangle();
		hover.FillColor = new Vector4(0, 0, 0, 0);
		grid.AddChild(hover);

		button.StateChange += sender => {
			hover.FillColor = sender.State switch
			{
				ButtonState.Normal => new Vector4(0, 0, 0, 0),
				ButtonState.Hovered => new Vector4(0, 0, 0, 0.2f),
				ButtonState.Pressed => new Vector4(0, 0, 0, 0.3f),
				_ => hover.FillColor
			};
		};
	
		return (button, image, textElement);
	}
}