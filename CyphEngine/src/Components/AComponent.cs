using System.Diagnostics;
using CyphEngine.Entities;
using CyphEngine.Rendering;
using CyphEngine.Scenes;
using JetBrains.Annotations;
using OpenTK.Mathematics;

namespace CyphEngine.Components;

[PublicAPI]
public abstract class AComponent
{
	internal Entity? EntityInternal { get; set; }
	public Entity Entity
	{
		get => EntityInternal ?? throw new InvalidOperationException();
		internal set => EntityInternal = value;
	}
	
	public Transform Transform => Entity.Transform;
	public Scene Scene => Entity.Scene;
	public Engine Engine => Scene.Engine;
	public Window Window => Engine.Window;

	public bool IsValid => EntityInternal != null;

	private bool _initialized = false;

	internal bool ComponentEnabled { get; set; } = true;
	public bool Enabled
	{
		get => ComponentEnabled && Entity.Enabled;
		set
		{
			ComponentEnabled = value;
			TriggerEnabledEvents();
		}
	}

	private bool _previousEnableValue = true;
	
	internal void TriggerEnabledEvents()
	{
		bool currentEnableValue = Enabled;
		if (_previousEnableValue != currentEnableValue)
		{
			_previousEnableValue = currentEnableValue;
			
			switch (currentEnableValue)
			{
				case true:
					OnEnable();
					break;
				case false:
					OnDisable();
					break;
			}
		}
	}

	internal void OnDestroyInternal()
	{
		Debug.Assert(IsValid);
		OnDestroy();
		EntityInternal = null;
	}

	internal void OnUpdateInternal(float deltaTime)
	{
		if (!_initialized)
		{
			OnInit();
			_initialized = true;
		}
		
		OnUpdate(deltaTime);
	}
	
	protected internal virtual void OnCreate() {}
	protected internal virtual void OnInit() {}
	protected internal virtual void OnDestroy() {}
	protected internal virtual void OnUpdate(float deltaTime) {}
	protected internal virtual void OnRender(Renderer renderer, ref Matrix4 viewProjection) {}
	protected internal virtual void OnCollide(PhysicsCollider thisPhysicsCollider, PhysicsCollider otherPhysicsCollider) {}
	protected internal virtual void OnEnable() {}
	protected internal virtual void OnDisable() {}
}