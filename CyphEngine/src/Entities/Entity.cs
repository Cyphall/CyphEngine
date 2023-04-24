using System.Collections.ObjectModel;
using System.Diagnostics;
using CyphEngine.Components;
using CyphEngine.Rendering;
using CyphEngine.Scenes;
using JetBrains.Annotations;
using OpenTK.Mathematics;

namespace CyphEngine.Entities;

[PublicAPI]
public sealed class Entity
{
	internal Scene? SceneInternal { get; set; }
	public Scene Scene
	{
		get => SceneInternal ?? throw new InvalidOperationException();
		internal set => SceneInternal = value;
	}
	
	public Transform Transform { get; }
	public string Name { get; set; }
	private List<AComponent> _components = new List<AComponent>();
	public ReadOnlyCollection<AComponent> Components => _components.AsReadOnly();

	private bool _enabled = true;
	public bool Enabled
	{
		get => _enabled;
		set
		{
			_enabled = value;
			
			for (int i = 0; i < _components.Count; i++)
			{
				_components[i].TriggerEnabledEvents();
			}
		}
	}

	public bool IsValid => SceneInternal != null;

	internal Entity(Transform parent, Scene scene, string name)
	{
		Transform = new Transform(this, parent);
		Scene = scene;
		Name = name;
	}

	public T CreateComponent<T>()
		where T: AComponent, new()
	{
		T component = new T();
		_components.Add(component);
		component.Entity = this;
		component.OnCreate();
		return component;
	}

	public T? GetComponent<T>()
		where T : AComponent
	{
		for (int i = 0; i < _components.Count; i++)
		{
			if (_components[i].GetType() == typeof(T))
			{
				return (T)_components[i];
			}
		}

		return null;
	}

	public List<T> GetComponents<T>()
		where T : AComponent
	{
		List<T> res = new List<T>();
		
		for (int i = 0; i < _components.Count; i++)
		{
			if (_components[i].GetType() == typeof(T))
			{
				res.Add((T)_components[i]);
			}
		}

		return res;
	}

	public void DestroyComponent(AComponent component)
	{
		Scene.ScheduleComponentDestroy(component);
	}
	
	internal void DoDestroyComponent(AComponent component)
	{
		component.OnDestroyInternal();
		_components.Remove(component);
	}

	internal void OnDestroyInternal()
	{
		Debug.Assert(IsValid);
		
		while (_components.Count > 0)
		{
			DoDestroyComponent(_components[^1]);
		}
		
		Transform.Dispose();

		SceneInternal = null;
	}

	internal void OnUpdate(float deltaTime)
	{
		for (int i = 0; i < _components.Count; i++)
		{
			if (_components[i].Enabled)
			{
				_components[i].OnUpdateInternal(deltaTime);
			}
		}
	}

	internal void OnRender(Renderer renderer, ref Matrix4 viewProjection)
	{
		for (int i = 0; i < _components.Count; i++)
		{
			if (_components[i].ComponentEnabled)
			{
				_components[i].OnRender(renderer, ref viewProjection);
			}
		}
	}

	internal void OnCollide(PhysicsCollider thisPhysicsCollider, PhysicsCollider otherPhysicsCollider)
	{
		for (int i = 0; i < _components.Count; i++)
		{
			if (_components[i].ComponentEnabled)
			{
				_components[i].OnCollide(thisPhysicsCollider, otherPhysicsCollider);
			}
		}
	}
}
