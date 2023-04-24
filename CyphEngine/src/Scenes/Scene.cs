using System.Diagnostics;
using CyphEngine.Components;
using CyphEngine.Entities;
using CyphEngine.Helper;
using CyphEngine.Rendering;
using CyphEngine.Resources;
using CyphEngine.UI;
using JetBrains.Annotations;
using OpenTK.Mathematics;

namespace CyphEngine.Scenes;

[PublicAPI]
public sealed class Scene
{
	public Transform Root { get; } = new Transform();
	private List<Entity> _entities = new List<Entity>();

	public UIManager UIManager { get; }

	private Engine? _engine;
	public Engine Engine => _engine ?? throw new InvalidOperationException();

	public bool IsValid => _engine != null;
	
	internal ResourceManager ResourceManager { get; } = new ResourceManager();

	private Camera? _nextMainCamera;
	public Camera? MainCamera { get; private set; }
	public bool HasScheduledCameraChange => _nextMainCamera != null;

	private List<Entity> _scheduledEntitiesToDestroy = new List<Entity>();
	private List<AComponent> _scheduledComponentsToDestroy = new List<AComponent>();

	private bool _timePaused;
	private bool _nextTimePauseState;

	private Dictionary<string, ulong> _physicsLayers = new Dictionary<string, ulong>();
	private Dictionary<ulong, ulong> _allowedCollisions = new Dictionary<ulong, ulong>();

	public ASceneMainScript? MainScript { get; private set; }

	internal Scene(Engine engine)
	{
		_engine = engine;
		UIManager = new UIManager(this);

		for (int i = 0; i < 64; i++)
		{
			_allowedCollisions.Add(1UL << i, 0);
		}
	}

	public void ScheduleCameraChange(Camera newCamera)
	{
		_nextMainCamera = newCamera;
	}

	public void PauseTime()
	{
		_nextTimePauseState = true;
	}

	public void UnpauseTime()
	{
		_nextTimePauseState = false;
	}

	public bool TimePaused => _timePaused;

	public ulong GetLayerFlag(string layerName)
	{
		if (!_physicsLayers.TryGetValue(layerName, out ulong layerFlag))
		{
			if (_physicsLayers.Count == 64)
				throw new OverflowException("All 64 physics layers are already in use. Cannot create a new one.");
			layerFlag = 1UL << _physicsLayers.Count;
			_physicsLayers.Add(layerName, layerFlag);
		}

		return layerFlag;
	}

	private static bool IsFlagValid(ulong flag)
	{
		for (int i = 0; i < 64; i++)
		{
			if ((flag ^ (1UL << i)) == 0)
			{
				return true;
			}
		}

		return false;
	}

	public void AllowCollisions(string layerName1, string layerName2)
	{
		ulong flag1 = GetLayerFlag(layerName1);
		ulong flag2 = GetLayerFlag(layerName2);
		
		_allowedCollisions[flag1] |= flag2;
		_allowedCollisions[flag2] |= flag1;
	}

	public void ForbidCollisions(string layerName1, string layerName2)
	{
		ulong flag1 = GetLayerFlag(layerName1);
		ulong flag2 = GetLayerFlag(layerName2);
		
		_allowedCollisions[flag1] &= ~flag2;
		_allowedCollisions[flag2] &= ~flag1;
	}

	public T CreateMainScript<T>()
		where T: ASceneMainScript, new()
	{
		T script = new T
		{
			Scene = this
		};
		MainScript = script;
		return script;
	}

	internal void OnUpdate(float deltaTime)
	{
		if (_nextMainCamera != null)
		{
			MainCamera = _nextMainCamera;
			_nextMainCamera = null;
		}

		_timePaused = _nextTimePauseState;

		if (_timePaused)
		{
			deltaTime = 0;
		}

		List<PhysicsCollider> physicsColliders = new List<PhysicsCollider>();
		for (int i = 0; i < _entities.Count; i++)
		{
			Entity entity = _entities[i];
			if (entity.Enabled)
			{
				for (int j = 0; j < entity.Components.Count; j++)
				{
					AComponent component = entity.Components[j];
					if (component.Enabled && component is PhysicsCollider physicsCollider && physicsCollider.Collider != null)
					{
						if (physicsCollider.LayerFlag != 0)
						{
							physicsColliders.Add(physicsCollider);
						}
						else
						{
							Console.WriteLine($"One of {physicsCollider.Entity.Name}'s colliders has no layer.");
						}
					}
				}
			}
		}
		
		List<Tuple<PhysicsCollider, PhysicsCollider>> collisions = new List<Tuple<PhysicsCollider, PhysicsCollider>>();
		for (int i = 0; i < physicsColliders.Count; i++)
		{
			for (int j = i+1; j < physicsColliders.Count; j++)
			{
				if ((_allowedCollisions[physicsColliders[i].LayerFlag] & physicsColliders[j].LayerFlag) > 0 && PhysicsHelper.Collides(physicsColliders[i].Collider!, physicsColliders[j].Collider!))
				{
					collisions.Add(new Tuple<PhysicsCollider, PhysicsCollider>(physicsColliders[i], physicsColliders[j]));
				}
			}
		}

		for (int i = 0; i < collisions.Count; i++)
		{
			PhysicsCollider physicsCollider1 = collisions[i].Item1;
			PhysicsCollider physicsCollider2 = collisions[i].Item2;
			
			physicsCollider1.Entity.OnCollide(physicsCollider1, physicsCollider2);
			physicsCollider2.Entity.OnCollide(physicsCollider2, physicsCollider1);
		}
		
		MainScript?.OnUpdate(deltaTime);
		
		for (int i = 0; i < _entities.Count; i++)
		{
			if (_entities[i].Enabled)
			{
				_entities[i].OnUpdate(deltaTime);
			}
		}
		
		UIManager.OnUpdate();
	}

	internal void OnRender(Renderer renderer)
	{
		MainScript?.OnRender(renderer);
		
		if (MainCamera != null)
		{
			Matrix4 viewProjection = MainCamera.View * MainCamera.Projection;
			for (int i = 0; i < _entities.Count; i++)
			{
				if (_entities[i].Enabled)
				{
					_entities[i].OnRender(renderer, ref viewProjection);
				}
			}
		}
		
		UIManager.OnRender(renderer);
	}

	internal void OnEndFrame()
	{
		for (int i = 0; i < _scheduledEntitiesToDestroy.Count; i++)
		{
			Entity entity = _scheduledEntitiesToDestroy[i];
			if (!entity.IsValid)
				continue;
			
			DoDestroyEntity(entity);
		}
		
		for (int i = 0; i < _scheduledComponentsToDestroy.Count; i++)
		{
			AComponent component = _scheduledComponentsToDestroy[i];
			if (!component.IsValid)
				continue;
			
			component.Entity.DoDestroyComponent(component);
		}
	}

	private void DoDestroyEntity(Entity entity)
	{
		while (entity.Transform.Children.Count > 0)
		{
			DoDestroyEntity(entity.Transform.Children[^1].Owner);
		}

		entity.OnDestroyInternal();
		_entities.RemoveLast(entity);
	}

	public Entity CreateEntity(IEntityPreset preset, Transform parent, string name = "New Entity")
	{
		Entity entity = new Entity(parent, this, name);
		_entities.Add(entity);
		preset.OnApply(entity);
		return entity;
	}

	internal void OnDestroyInternal()
	{
		Debug.Assert(IsValid);
		
		while (_entities.Count > 0)
		{
			DoDestroyEntity(_entities[^1]);
		}
		
		ResourceManager.Dispose();

		_engine = null;
	}

	internal void ScheduleEntityDestroy(Entity entity)
	{
		_scheduledEntitiesToDestroy.Add(entity);
	}

	internal void ScheduleComponentDestroy(AComponent component)
	{
		_scheduledComponentsToDestroy.Add(component);
	}

	public void DestroyEntity(Entity entity)
	{
		ScheduleEntityDestroy(entity);
	}

	public T? FindComponentByType<T>()
		where T: AComponent
	{
		for (int i = 0; i < _entities.Count; i++)
		{
			for (int j = 0; j < _entities[i].Components.Count; j++)
			{
				if (_entities[i].Components[j] is T component)
				{
					return component;
				}
			}
		}

		return null;
	}
}