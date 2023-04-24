using System.Collections.ObjectModel;
using CyphEngine.Rendering;
using JetBrains.Annotations;
using OpenTK.Mathematics;

namespace CyphEngine.UI;

[PublicAPI]
public abstract class AUIMultiContainer : AUIElement
{
	private List<AUIElement> _children = new List<AUIElement>();
	public ReadOnlyCollection<AUIElement> Children => _children.AsReadOnly();
	
	public void AddChild(AUIElement child)
	{
		if (GetParent(child) != null)
		{
			throw new InvalidOperationException("This element is already part of the UI tree.");
		}

		SetParent(child, this);
		
		_children.Add(child);
		Manager.SetDirty();
		
		OnChildAdded(child);
	}

	public void RemoveChild(AUIElement child)
	{
		SetParent(child, null);
		
		_children.Remove(child);
		Manager.SetDirty();
		
		OnChildRemoved(child);
	}

	protected virtual void OnChildAdded(AUIElement child) {}
	protected virtual void OnChildRemoved(AUIElement child) {}
	
	protected override void UpdateOverride()
	{
		for (int i = 0; i < _children.Count; i++)
		{
			_children[i].Update();
		}
	}
	
	protected override void RenderOverride(Renderer renderer, ref Matrix4 projection)
	{
		base.RenderOverride(renderer, ref projection);
		
		for (int i = 0; i < _children.Count; i++)
		{
			_children[i].Render(renderer, ref projection);
		}
	}
}