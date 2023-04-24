using CyphEngine.Maths;
using JetBrains.Annotations;
using OpenTK.Mathematics;

namespace CyphEngine.UI;

[PublicAPI]
public class UICanvas : AUIMultiContainer
{
	private class PositionData
	{
		public Anchor Anchor;
		public Vector2 Position;
	}
	
	private Dictionary<AUIElement, PositionData> _positionData = new Dictionary<AUIElement, PositionData>();

	protected override Vector2 MeasureOverride(Vector2 availableSize)
	{
		foreach (AUIElement child in Children)
		{
			child.Measure(new Vector2(float.PositiveInfinity, float.PositiveInfinity));
		}

		return Vector2.Zero;
	}

	protected override void ArrangeOverride(Rect finalRect)
	{
		foreach (AUIElement child in Children)
		{
			PositionData positionData = _positionData[child];

			Vector2 weight;
			
			switch (positionData.Anchor)
			{
				case Anchor.TopLeft:
					weight = new Vector2(0.0f, 0.0f);
					break;
				case Anchor.Top:
					weight = new Vector2(0.5f, 0.0f);
					break;
				case Anchor.TopRight:
					weight = new Vector2(1.0f, 0.0f);
					break;
				case Anchor.Right:
					weight = new Vector2(1.0f, 0.5f);
					break;
				case Anchor.BottomRight:
					weight = new Vector2(1.0f, 1.0f);
					break;
				case Anchor.Bottom:
					weight = new Vector2(0.5f, 1.0f);
					break;
				case Anchor.BottomLeft:
					weight = new Vector2(0.0f, 1.0f);
					break;
				case Anchor.Left:
					weight = new Vector2(0.0f, 0.5f);
					break;
				case Anchor.Center:
					weight = new Vector2(0.5f, 0.5f);
					break;
				default:
					throw new InvalidOperationException();
			}
			
			Vector2 offset = child.DesiredBoundingBoxSize * weight;
			Vector2 origin = finalRect.Size * weight;
			
			child.Arrange(Rect.FromOriginSize(finalRect.Min + origin + positionData.Position - offset, child.DesiredBoundingBoxSize));
		}
	}

	public Vector2 GetChildPosition(AUIElement child)
	{
		if (!_positionData.TryGetValue(child, out PositionData? data))
		{
			throw new InvalidOperationException();
		}
		
		return data.Position;
	}

	public void SetChildPosition(AUIElement child, Vector2 position)
	{
		if (!_positionData.ContainsKey(child))
		{
			throw new InvalidOperationException();
		}
		
		_positionData[child].Position = position;
		
		Manager.SetDirty();
	}

	public Anchor GetChildAnchor(AUIElement child)
	{
		if (!_positionData.TryGetValue(child, out PositionData? data))
		{
			throw new InvalidOperationException();
		}
		
		return data.Anchor;
	}

	public void SetChildAnchor(AUIElement child, Anchor anchor)
	{
		if (!_positionData.ContainsKey(child))
		{
			throw new InvalidOperationException();
		}
		
		_positionData[child].Anchor = anchor;
		
		Manager.SetDirty();
	}

	protected override void OnChildAdded(AUIElement child)
	{
		_positionData[child] = new PositionData
		{
			Anchor = Anchor.TopLeft,
			Position = new Vector2(0, 0)
		};
	}

	protected override void OnChildRemoved(AUIElement child)
	{
		_positionData.Remove(child);
	}
}