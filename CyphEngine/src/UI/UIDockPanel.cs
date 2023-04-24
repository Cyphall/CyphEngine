using CyphEngine.Maths;
using JetBrains.Annotations;
using OpenTK.Mathematics;

namespace CyphEngine.UI;

[PublicAPI]
public class UIDockPanel : AUIMultiContainer
{
	private AUIElement?[] _children = new AUIElement?[5];
	private DockSide[] _dockOrder = { DockSide.Top, DockSide.Right, DockSide.Bottom, DockSide.Left};

	private bool _addingChildWithDock;

	protected override Vector2 MeasureOverride(Vector2 availableSize)
	{
		Vector2 requiredSize = Vector2.Zero;
		Vector2 accumulatedSize = Vector2.Zero;

		for (int i = 0; i < 5; i++)
		{
			DockSide dockSide = i == 4 ? (DockSide)4 : _dockOrder[i];
			
			AUIElement? child = _children[(int)dockSide];

			if (child == null)
			{
				continue;
			}

			child.Measure(Vector2.ComponentMax(Vector2.Zero, availableSize - accumulatedSize));
			Vector2 desiredSize = child.DesiredBoundingBoxSize;
			
			switch (dockSide)
			{
				case DockSide.Left:
				case DockSide.Right:
				default:
				{
					accumulatedSize.X += desiredSize.X;
					requiredSize.Y = MathF.Max(requiredSize.Y, desiredSize.Y + accumulatedSize.Y);
					break;
				}
				case DockSide.Top:
				case DockSide.Bottom:
				{
					accumulatedSize.Y += desiredSize.Y;
					requiredSize.X = MathF.Max(requiredSize.X, desiredSize.X + accumulatedSize.X);
					break;
				}
			}
		}

		requiredSize = Vector2.ComponentMax(requiredSize, accumulatedSize);
		
		return requiredSize;
	}

	protected override void ArrangeOverride(Rect finalRect)
	{
		for (int i = 0; i < 4; i++)
		{
			int dockIndex = (int)_dockOrder[i];
			
			AUIElement? child = _children[dockIndex];
			
			if (child != null)
			{
				Vector2 size = child.DesiredBoundingBoxSize;
				
				switch (_dockOrder[i])
				{
					case DockSide.Top:
					{
						Vector2 origin = finalRect.Min;
						child.Arrange(Rect.FromOriginSize(origin, new Vector2(finalRect.Size.X, size.Y)));
						
						finalRect.Min.Y += size.Y;
						break;
					}
					case DockSide.Right:
					{
						Vector2 origin = new Vector2(finalRect.Max.X - size.X, finalRect.Min.Y);
						child.Arrange(Rect.FromOriginSize(origin, new Vector2(size.X, finalRect.Size.Y)));
						
						finalRect.Max.X -= size.X;
						break;
					}
					case DockSide.Bottom:
					{
						Vector2 origin = new Vector2(finalRect.Min.X, finalRect.Max.Y - size.Y);
						child.Arrange(Rect.FromOriginSize(origin, new Vector2(finalRect.Size.X, size.Y)));
						
						finalRect.Max.Y -= size.Y;
						break;
					}
					case DockSide.Left:
					{
						Vector2 origin = finalRect.Min;
						child.Arrange(Rect.FromOriginSize(origin, new Vector2(size.X, finalRect.Size.Y)));
						
						finalRect.Min.X += size.X;
						break;
					}
				}
			}
		}
		
		_children[4]?.Arrange(finalRect);
	}
	
	public void AddChild(AUIElement child, DockSide dockSide)
	{
		_addingChildWithDock = true;
		AddChild(child);
		_addingChildWithDock = false;
		
		_children[(int)dockSide] = child;
	}

	public void SetDockOrder(DockSide first, DockSide second, DockSide third, DockSide fourth)
	{
		_dockOrder[0] = first;
		_dockOrder[1] = second;
		_dockOrder[2] = third;
		_dockOrder[3] = fourth;
	}

	protected override void OnChildAdded(AUIElement child)
	{
		if (Children.Count > 5)
		{
			throw new InvalidOperationException();
		}

		if (!_addingChildWithDock)
		{
			_children[4] = child;
		}
	}

	protected override void OnChildRemoved(AUIElement child)
	{
		for (int i = 0; i < _children.Length; i++)
		{
			if (_children[i] == child)
			{
				_children[i] = null;
				break;
			}
		}
	}
}