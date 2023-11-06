using CyphEngine.Maths;
using JetBrains.Annotations;
using OpenTK.Mathematics;

namespace CyphEngine.UI;

[PublicAPI]
public class UIStackPanel : AUIMultiContainer
{
	private Direction _orientation = Direction.TopToBottom;

	public Direction Direction
	{
		get => _orientation;
		set
		{
			_orientation = value;
			Manager.SetDirty();
		}
	}

	protected override Vector2 MeasureOverride(Vector2 availableSize)
	{
		Vector2 actuallyAvailableSize = availableSize;
		switch (Direction)
		{
			case Direction.LeftToRight:
			case Direction.RightToLeft:
				actuallyAvailableSize.X = float.PositiveInfinity;
				break;
			case Direction.TopToBottom:
			case Direction.BottomToTop:
				actuallyAvailableSize.Y = float.PositiveInfinity;
				break;
		}

		float flowSize = 0;
		float widthSize = 0;

		AUIElement? previousChild = null;

		for (int i = 0; i < Children.Count; i++)
		{
			AUIElement child = Children[i];

			child.Measure(actuallyAvailableSize);

			if (previousChild != null)
			{
				float margin = MathF.Max(previousChild.Margin, child.Margin);
				flowSize -= margin;
			}

			switch (Direction)
			{
				case Direction.LeftToRight:
				case Direction.RightToLeft:
					flowSize += child.DesiredBoundingBoxSize.X;
					widthSize = Math.Max(widthSize, child.DesiredBoundingBoxSize.Y);
					break;
				case Direction.TopToBottom:
				case Direction.BottomToTop:
					flowSize += child.DesiredBoundingBoxSize.Y;
					widthSize = Math.Max(widthSize, child.DesiredBoundingBoxSize.X);
					break;
			}

			previousChild = child;
		}

		Vector2 size = Vector2.Zero;
		switch (Direction)
		{
			case Direction.LeftToRight:
			case Direction.RightToLeft:
				size = new Vector2(flowSize, widthSize);
				break;
			case Direction.TopToBottom:
			case Direction.BottomToTop:
				size = new Vector2(widthSize, flowSize);
				break;
		}

		return Vector2.ComponentMin(size, availableSize);
	}

	protected override void ArrangeOverride(Rect finalRect)
	{
		float cursor = 0;
		AUIElement? previousChild = null;
		
		for (int i = 0; i < Children.Count; i++)
		{
			AUIElement child = Children[i];
			
			if (previousChild != null)
			{
				float margin = MathF.Max(previousChild.Margin, child.Margin);
				cursor -= margin;
			}
			
			switch (Direction)
			{
				case Direction.LeftToRight:
				{
					Vector2 size = new Vector2(child.DesiredBoundingBoxSize.X, finalRect.Size.Y);
					Vector2 topLeft = finalRect.TopLeft + new Vector2(cursor, 0);
					
					child.Arrange(Rect.FromOriginSize(topLeft, size));
					
					cursor += child.DesiredBoundingBoxSize.X;
					break;
				}
				case Direction.RightToLeft:
				{
					cursor += child.DesiredBoundingBoxSize.X;
					
					Vector2 size = new Vector2(child.DesiredBoundingBoxSize.X, finalRect.Size.Y);
					Vector2 topLeft = finalRect.TopRight - new Vector2(cursor, 0);
					
					child.Arrange(Rect.FromOriginSize(topLeft, size));
					break;
				}
				case Direction.TopToBottom:
				{
					Vector2 size = new Vector2(finalRect.Size.X, child.DesiredBoundingBoxSize.Y);
					Vector2 topLeft = finalRect.TopLeft + new Vector2(0, cursor);
					
					child.Arrange(Rect.FromOriginSize(topLeft, size));
					
					cursor += child.DesiredBoundingBoxSize.Y;
					break;
				}
				case Direction.BottomToTop:
				{
					cursor += child.DesiredBoundingBoxSize.Y;
					
					Vector2 size = new Vector2(finalRect.Size.X, child.DesiredBoundingBoxSize.Y);
					Vector2 topLeft = finalRect.BottomLeft - new Vector2(0, cursor);
					
					child.Arrange(Rect.FromOriginSize(topLeft, size));
					break;
				}
			}

			previousChild = child;
		}
	}
}