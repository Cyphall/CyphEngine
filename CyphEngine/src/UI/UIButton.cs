using CyphEngine.Maths;
using JetBrains.Annotations;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace CyphEngine.UI;

[PublicAPI]
public class UIButton : AUISingleContainer
{
	#region Events

	public event Action<UIButton>? Click;
	private void OnClick()
	{
		Click?.Invoke(this);
	}
	
	public event Action<UIButton>? StateChange;
	private void OnStateChange()
	{
		StateChange?.Invoke(this);
	}
	
	#endregion

	private bool _wasPressed;

	private ButtonState _state = ButtonState.Normal;
	public ButtonState State
	{
		get => _state;
		private set
		{
			if (_state != value)
			{
				_state = value;
				OnStateChange();
			}
		}
	}

	protected override Vector2 MeasureOverride(Vector2 availableSize)
	{
		Child?.Measure(availableSize);
		return Vector2.ComponentMin(Child?.DesiredBoundingBoxSize ?? Vector2.Zero, availableSize);
	}

	protected override void ArrangeOverride(Rect finalRect)
	{
		Child?.Arrange(finalRect);
	}

	protected override void UpdateOverride()
	{
		ButtonState state = ButtonState.Normal;
		
		bool isOver = Rect.FromOriginSize(ContentPosition, ActualContentSize).Contains(Window.CursorPos);

		if (isOver)
		{
			Manager.Cursor = CursorShape.Hand;
			state = ButtonState.Hovered;
		}
		
		if (!_wasPressed && Window.MouseButtonPressed(MouseButton.Left) && isOver)
		{
			_wasPressed = true;
		}
		else if (_wasPressed && Window.MouseButtonReleased(MouseButton.Left) && isOver)
		{
			OnClick();
			_wasPressed = false;
		}

		if (_wasPressed)
		{
			state = ButtonState.Pressed;
		}

		State = state;
		
		base.UpdateOverride();
	}
}