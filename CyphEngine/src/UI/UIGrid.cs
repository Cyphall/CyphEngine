using CyphEngine.Maths;
using OpenTK.Mathematics;

namespace CyphEngine.UI;

public class UIGrid : AUIMultiContainer
{
	private int _columns = 1;
	private int _rows = 1;
	
	private Dictionary<AUIElement, Vector2i> _positions = new Dictionary<AUIElement, Vector2i>();

	private float[] _columnsOffset = null!;
	private float[] _rowsOffset = null!;
	
	private float[] _columnsSize = null!;
	private float[] _rowsSize = null!;

	public int Columns
	{
		get => _columns;
		set
		{
			if (value < 1)
			{
				throw new InvalidOperationException("A grid cannot have less than 1 column.");
			}

			_columns = value;

			foreach ((AUIElement key, Vector2i position) in _positions)
			{
				if (position.X >= _columns)
				{
					_positions[key] = new Vector2i(0, position.Y);
				}
			}
		}
	}

	public int Rows
	{
		get => _rows;
		set
		{
			if (value < 1)
			{
				throw new InvalidOperationException("A grid cannot have less than 1 row.");
			}

			_rows = value;

			foreach ((AUIElement key, Vector2i position) in _positions)
			{
				if (position.Y >= _rows)
				{
					_positions[key] = new Vector2i(position.X, 0);
				}
			}
		}
	}

	public void SetChildPosition(AUIElement child, Vector2i position)
	{
		if (!_positions.ContainsKey(child))
		{
			throw new InvalidOperationException("This element is not a child of this grid.");
		}

		if (position.X < 0 || position.X >= _columns)
		{
			throw new InvalidOperationException("X position is not valid.");
		}
		
		if (position.Y < 0 || position.Y >= _rows)
		{
			throw new InvalidOperationException("Y position is not valid.");
		}

		_positions[child] = position;
	}
	
	protected override void OnChildAdded(AUIElement child)
	{
		_positions[child] = new Vector2i(0, 0);
	}

	protected override void OnChildRemoved(AUIElement child)
	{
		_positions.Remove(child);
	}

	protected override Vector2 MeasureOverride(Vector2 availableSize)
	{
		_columnsSize = new float[_columns];
		_rowsSize = new float[_rows];
		
		_columnsOffset = new float[_columns];
		_rowsOffset = new float[_rows];
		
		foreach ((AUIElement element, Vector2i position) in _positions)
		{
			element.Measure(availableSize);
			
			Vector2 desiredSize = element.DesiredBoundingBoxSize;
			_columnsSize[position.X] = MathF.Max(_columnsSize[position.X], desiredSize.X);
			_rowsSize[position.Y] = MathF.Max(_rowsSize[position.Y], desiredSize.Y);
		}
		
		for (int i = 1; i < _columnsSize.Length; i++)
		{
			_columnsOffset[i] = _columnsOffset[i - 1] + _columnsSize[i - 1];
		}
		for (int i = 1; i < _rowsSize.Length; i++)
		{
			_rowsOffset[i] = _rowsOffset[i - 1] + _rowsSize[i - 1];
		}
		
		Vector2 requiredSize = Vector2.Zero;
		
		for (int i = 0; i < _columnsSize.Length; i++)
		{
			requiredSize.X += _columnsSize[i];
		}
		for (int i = 0; i < _rowsSize.Length; i++)
		{
			requiredSize.Y += _rowsSize[i];
		}

		return Vector2.ComponentMin(requiredSize, availableSize);
	}

	protected override void ArrangeOverride(Rect finalRect)
	{
		Vector2 gridScale = finalRect.Size / DesiredContentSize;
		
		foreach ((AUIElement element, Vector2i position) in _positions)
		{
			Vector2 offset = new Vector2
			{
				X = _columnsOffset[position.X],
				Y = _rowsOffset[position.Y]
			} * gridScale;
			
			Vector2 size = new Vector2
			{
				X = _columnsSize[position.X],
				Y = _rowsSize[position.Y]
			} * gridScale;
			
			element.Arrange(Rect.FromOriginSize(finalRect.Min + offset, size));
		}
	}
}