using CyphEngine.Maths;
using CyphEngine.Rendering;
using JetBrains.Annotations;
using OpenTK.Mathematics;

namespace CyphEngine.UI;

[PublicAPI]
public abstract class AUISingleContainer : AUIElement
{
	private AUIElement? _child;
	public AUIElement? Child
	{
		get => _child;
		set
		{
			if (value != null && GetParent(value) != null)
			{
				throw new InvalidOperationException("This element is already part of the UI tree.");
			}

			AUIElement? previousChild = _child;

			if (_child != null)
			{
				SetParent(_child, null);
			}

			_child = value;
			
			if (_child != null)
			{
				SetParent(_child, this);
			}
			
			OnChildChanged(previousChild, _child);
			
			Manager.SetDirty();
		}
	}

	protected override void UpdateOverride()
	{
		Child?.Update();
	}

	protected override void RenderOverride(Renderer renderer, ref Matrix4 projection)
	{
		base.RenderOverride(renderer, ref projection);

		renderer.PushUIScissorArea(Rect.FromOriginSize(BoundingBoxPosition, ActualBoundingBoxSize));
		Child?.Render(renderer, ref projection);
		renderer.PopUIScissorArea();
	}
	
	protected virtual void OnChildChanged(AUIElement? previousChild, AUIElement? newChild) {}
}