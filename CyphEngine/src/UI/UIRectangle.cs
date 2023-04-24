using CyphEngine.Maths;
using CyphEngine.Rendering;
using JetBrains.Annotations;
using OpenTK.Mathematics;

namespace CyphEngine.UI;

[PublicAPI]
public class UIRectangle : AUISingleContainer
{
	private float _cornerRadius;

	public float CornerRadius
	{
		get => _cornerRadius;
		set
		{
			_cornerRadius = value;
			Manager.SetDirty();
		}
	}

	public Vector4 FillColor { get; set; }
	public Vector4 BorderColor { get; set; }
	
	public float BorderThickness { get; set; }

	protected override Vector2 MeasureOverride(Vector2 availableSize)
	{
		Vector2 requiredSize = new Vector2(BorderThickness, BorderThickness) * 2;
		
		if (Child != null)
		{
			Child.Measure(availableSize - requiredSize);
			requiredSize += Child.DesiredBoundingBoxSize;
		}
		
		return Vector2.ComponentMin(requiredSize, availableSize);
	}

	protected override void ArrangeOverride(Rect finalRect)
	{
		if (Child != null)
		{
			Vector2 borderSize = new Vector2(BorderThickness, BorderThickness);
	
			Child.Arrange(Rect.FromTwoPoints(finalRect.Min + borderSize, finalRect.Max - borderSize));
		}
	}

	protected override void RenderOverride(Renderer renderer, ref Matrix4 projection)
	{
		Matrix4 matrix = Matrix4.CreateScale(ActualContentWidth, ActualContentHeight, 1) * Matrix4.CreateTranslation(ContentX, ContentY, 0) * projection;
		
		renderer.AddUIRectangleRequest(FillColor, BorderColor, matrix, CornerRadius, ActualContentSize, BorderThickness);
		
		base.RenderOverride(renderer, ref projection);
	}
}