using CyphEngine.Maths;
using OpenTK.Mathematics;

namespace CyphEngine.UI;

public class UIRoot : AUISingleContainer
{
	protected override Vector2 MeasureOverride(Vector2 availableSize)
	{
		Vector2 size = Vector2.Zero;

		if (Child != null)
		{
			Child.Measure(availableSize);
			size = Vector2.ComponentMin(size, Child.DesiredBoundingBoxSize);
		}

		return size;
	}

	protected override void ArrangeOverride(Rect finalRect)
	{
		Child?.Arrange(finalRect);
	}
}