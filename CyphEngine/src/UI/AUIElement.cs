using CyphEngine.Maths;
using CyphEngine.Rendering;
using CyphEngine.Scenes;
using JetBrains.Annotations;
using OpenTK.Mathematics;

namespace CyphEngine.UI;

[PublicAPI]
public abstract class AUIElement
{
	public UIManager Manager { get; private set; }
	public Scene Scene => Manager.Scene;
	public Engine Engine => Scene.Engine;
	public Window Window => Engine.Window;

	private AUIElement? _parent;

	private Vector2 _desiredBoundingBoxSize;
	private Vector2 _desiredContentSize;
	
	private Vector2 _actualBoundingBoxSize;
	private Vector2 _actualContentSize;
	
	private Vector2 _boundingBoxPosition;
	private Vector2 _contentPosition;
	
	private Vector2 _size = new Vector2(float.NaN, float.NaN);
	
	private Visibility _visibility = Visibility.Visible;

	private VerticalAlignment _verticalAlignment = VerticalAlignment.Stretch;
	private HorizontalAlignment _horizontalAlignment = HorizontalAlignment.Stretch;

	private float _margin;

	public Vector2 DesiredBoundingBoxSize
	{
		get => _desiredBoundingBoxSize;
		private set => _desiredBoundingBoxSize = value;
	}
	public float DesiredBoundingBoxWidth => _desiredBoundingBoxSize.X;
	public float DesiredBoundingBoxHeight => _desiredBoundingBoxSize.Y;

	public Vector2 DesiredContentSize
	{
		get => _desiredContentSize;
		private set => _desiredContentSize = value;
	}
	public float DesiredContentWidth => _desiredContentSize.X;
	public float DesiredContentHeight => _desiredContentSize.Y;

	public Vector2 ActualBoundingBoxSize
	{
		get => _actualBoundingBoxSize;
		private set => _actualBoundingBoxSize = value;
	}
	public float ActualBoundingBoxWidth => _actualBoundingBoxSize.X;
	public float ActualBoundingBoxHeight => _actualBoundingBoxSize.Y;

	public Vector2 ActualContentSize
	{
		get => _actualContentSize;
		private set => _actualContentSize = value;
	}
	public float ActualContentWidth => _actualContentSize.X;
	public float ActualContentHeight => _actualContentSize.Y;

	public Vector2 Size
	{
		get => _size;
		set
		{
			_size = value;
			Manager.SetDirty();
		}
	}
	public float Width
	{
		get => _size.X;
		set
		{
			_size.X = value;
			Manager.SetDirty();
		}
	}
	public float Height
	{
		get => _size.Y;
		set
		{
			_size.Y = value;
			Manager.SetDirty();
		}
	}

	public Vector2 BoundingBoxPosition
	{
		get => _boundingBoxPosition;
		private set => _boundingBoxPosition = value;
	}
	public float BoundingBoxX => _boundingBoxPosition.X;
	public float BoundingBoxY => _boundingBoxPosition.Y;

	public Vector2 ContentPosition
	{
		get => _contentPosition;
		private set => _contentPosition = value;
	}
	public float ContentX => _contentPosition.X;
	public float ContentY => _contentPosition.Y;

	public VerticalAlignment VerticalAlignment
	{
		get => _verticalAlignment;
		set
		{
			_verticalAlignment = value;
			Manager.SetDirty();
		}
	}
	public HorizontalAlignment HorizontalAlignment
	{
		get => _horizontalAlignment;
		set
		{
			_horizontalAlignment = value;
			Manager.SetDirty();
		}
	}

	public Visibility Visibility
	{
		get => _visibility;
		set
		{
			_visibility = value;
			Manager.SetDirty();
		}
	}

	public float Margin
	{
		get => _margin;
		set
		{
			_margin = value;
			Manager.SetDirty();
		}
	}

	public bool DebugDrawBoundingBox { get; set; }

	public Vector4 DebugBoundingBoxColor { get; set; } = new Vector4(0, 1, 0, 1);

	protected AUIElement()
	{
		Manager = UIManager.Current;
	}

	protected internal void Measure(Vector2 availableSize)
	{
		if (Visibility == Visibility.Collapsed)
		{
			DesiredBoundingBoxSize = Vector2.Zero;
			DesiredContentSize = Vector2.Zero;
			return;
		}

		Vector2 marginTotalSize = new Vector2(Margin) * 2;
		
		Vector2 availableContentSize = Vector2.ComponentMax(availableSize - marginTotalSize, Vector2.Zero);
		
		Vector2 minContentSize = new Vector2();
		Vector2 maxContentSize = new Vector2();

		if (float.IsNaN(Size.X))
		{
			minContentSize.X = 0;
			maxContentSize.X = availableContentSize.X;
		}
		else
		{
			float constrainedContentSize = Math.Clamp(Size.X, 0, availableContentSize.X);
			minContentSize.X = constrainedContentSize;
			maxContentSize.X = constrainedContentSize;
		}

		if (float.IsNaN(Size.Y))
		{
			minContentSize.Y = 0;
			maxContentSize.Y = availableContentSize.Y;
		}
		else
		{
			float constrainedContentSize = Math.Clamp(Size.Y, 0, availableContentSize.Y);
			minContentSize.Y = constrainedContentSize;
			maxContentSize.Y = constrainedContentSize;
		}

		Vector2 desiredContentSize = MeasureOverride(maxContentSize);

		if (desiredContentSize.X > maxContentSize.X || desiredContentSize.Y > maxContentSize.Y)
		{
			throw new InvalidOperationException("The size returned by MeasureOverride must not be greater than availableSize.");
		}

		if (desiredContentSize.X < 0 || desiredContentSize.Y < 0)
		{
			throw new InvalidOperationException("The size returned by MeasureOverride must not be less than 0.");
		}

		DesiredContentSize = Vector2.Clamp(desiredContentSize, minContentSize, maxContentSize);

		DesiredBoundingBoxSize = DesiredContentSize + marginTotalSize;
	}
	
	protected internal void Arrange(Rect finalRect)
	{
		Vector2 size;
		Vector2 relativePosition;
		

		HorizontalAlignment actualHorizontalAlignment = HorizontalAlignment;
		if (actualHorizontalAlignment == HorizontalAlignment.Stretch && !float.IsNaN(Size.X))
		{
			actualHorizontalAlignment = HorizontalAlignment.Center;
		}
		
		switch (actualHorizontalAlignment)
		{
			case HorizontalAlignment.Left:
			case HorizontalAlignment.Center:
			case HorizontalAlignment.Right:
				size.X = Math.Min(DesiredBoundingBoxSize.X, finalRect.Size.X);
				break;
			case HorizontalAlignment.Stretch:
				size.X = finalRect.Size.X;
				break;
			default:
				throw new InvalidOperationException();
		}

		switch (actualHorizontalAlignment)
		{
			case HorizontalAlignment.Left:
			case HorizontalAlignment.Stretch:
				relativePosition.X = 0;
				break;
			case HorizontalAlignment.Center:
				relativePosition.X = (finalRect.Size.X - size.X) / 2;
				break;
			case HorizontalAlignment.Right:
				relativePosition.X = finalRect.Size.X - size.X;
				break;
			default:
				throw new InvalidOperationException();
		}
		

		VerticalAlignment actualVerticalAlignment = VerticalAlignment;
		if (actualVerticalAlignment == VerticalAlignment.Stretch && !float.IsNaN(Size.Y))
		{
			actualVerticalAlignment = VerticalAlignment.Center;
		}
		
		switch (actualVerticalAlignment)
		{
			case VerticalAlignment.Top:
			case VerticalAlignment.Center:
			case VerticalAlignment.Bottom:
				size.Y = Math.Min(DesiredBoundingBoxSize.Y, finalRect.Size.Y);
				break;
			case VerticalAlignment.Stretch:
				size.Y = finalRect.Size.Y;
				break;
			default:
				throw new InvalidOperationException();
		}

		switch (actualVerticalAlignment)
		{
			case VerticalAlignment.Top:
			case VerticalAlignment.Stretch:
				relativePosition.Y = 0;
				break;
			case VerticalAlignment.Center:
				relativePosition.Y = (finalRect.Size.Y - size.Y) / 2;
				break;
			case VerticalAlignment.Bottom:
				relativePosition.Y = finalRect.Size.Y - size.Y;
				break;
			default:
				throw new InvalidOperationException();
		}

		BoundingBoxPosition = finalRect.Min + relativePosition;
		ActualBoundingBoxSize = size;
		
		Vector2 marginSize = new Vector2(Margin, Margin);
		ContentPosition = BoundingBoxPosition + marginSize;
		ActualContentSize = Vector2.ComponentMax(ActualBoundingBoxSize - marginSize*2, Vector2.Zero);

		ArrangeOverride(Rect.FromOriginSize(ContentPosition, ActualContentSize));
	}

	protected internal void Update()
	{
		UpdateOverride();
	}

	protected internal void Render(Renderer renderer, ref Matrix4 projection)
	{
		if (Visibility == Visibility.Visible)
		{
			RenderOverride(renderer, ref projection);
		}
	}
	
	protected static void SetParent(AUIElement @this, AUIElement? parent)
	{
		@this._parent = parent;
	}
	
	protected static AUIElement? GetParent(AUIElement @this)
	{
		return @this._parent;
	}
	
	protected virtual Vector2 MeasureOverride(Vector2 availableSize)
	{
		return Vector2.Zero;
	}

	protected virtual void ArrangeOverride(Rect finalRect)
	{
		
	}

	protected virtual void UpdateOverride()
	{
		
	}

	protected virtual void RenderOverride(Renderer renderer, ref Matrix4 projection)
	{
		if (DebugDrawBoundingBox)
		{
			Matrix4 matrix = Matrix4.CreateScale(ActualBoundingBoxWidth, ActualBoundingBoxHeight, 1) * Matrix4.CreateTranslation(BoundingBoxX + ActualBoundingBoxWidth/2, BoundingBoxY + ActualBoundingBoxHeight/2, 0) * projection;
		
			renderer.AddWireframeBoxRequest(DebugBoundingBoxColor, matrix);
		}
	}
}