using System.Diagnostics;
using CyphEngine.Helper;
using JetBrains.Annotations;
using OpenTK.Mathematics;
using MathHelper = CyphEngine.Helper.MathHelper;
using TKMathHelper = OpenTK.Mathematics.MathHelper;

namespace CyphEngine.Entities;

[PublicAPI]
public sealed class Transform : IDisposable
{
	public Transform(Entity owner, Transform parent)
	{
		Owner = owner;
		Parent = parent;
	}
	
	internal Transform() // Only for root component
	{
		
	}

	public void Dispose()
	{
		if (_parent == null) // this == Scene.Root
		{
			Debug.Assert(Children.Count == 0, "Root Transform is disposed while it still contains children.");
		}
		else
		{
			Parent.Children.RemoveLast(this);
			
			while (Children.Count > 0)
			{
				Children[^1].Parent = Parent;
			}
		}
	}
	
	private Transform? _parent;
	public Transform Parent
	{
		get => _parent ?? throw new InvalidOperationException("Cannot get parent of root component");
		set
		{
			if (value == _parent) return;
			if (value == this) throw new ArgumentException("Cannot set Transform's parent to itself");
			if (value == null) throw new ArgumentNullException(nameof(value), "Cannot remove Transform's parent, only changing it is allowed");

			_parent?.Children.RemoveLast(this);
			_parent = value;
			_parent.Children.Add(this);
		
			InvalidateWorldCache();
		}
	}

	public List<Transform> Children { get; } = new List<Transform>();
	
	private bool _invalidLocalCache = true;
	private bool _invalidWorldCache = true;

	private Entity? _owner;
	public Entity Owner
	{
		get => _owner ?? throw new InvalidOperationException("Cannot get owner of root component");
		private set => _owner = value;
	}

	#region Position
	
	private Vector2 _localPosition;
	public Vector2 LocalPosition
	{
		get => _localPosition;
		set
		{
			if (value == _localPosition) return;
		
			_localPosition = value;
			InvalidateLocalCache();
		}
	}
	
	private Vector2 _cachedWorldPosition;
	public Vector2 WorldPosition
	{
		get
		{
			if (_invalidWorldCache)
			{
				RecalculateWorldCache();
			}
			return _cachedWorldPosition;
		}
		private set => _cachedWorldPosition = value;
	}
	
	#endregion
	#region Rotation
	
	private float _localRotation;
	public float LocalRotation
	{
		get => _localRotation;
		set
		{
			if (value == _localRotation) return;
		
			_localRotation = CorrectRotation(value);
			InvalidateLocalCache();
		}
	}

	private float _cachedWorldRotation;
	public float WorldRotation
	{
		get
		{
			if (_invalidWorldCache)
			{
				RecalculateWorldCache();
			}
			return _cachedWorldRotation;
		}
		private set => _cachedWorldRotation = value;
	}
	
	#endregion
	
	#region Matrix
	private Matrix4 _cachedLocalToParentMatrix;
	public Matrix4 LocalToParentMatrix
	{
		get
		{
			if (_invalidLocalCache)
			{
				RecalculateLocalCache();
			}
		
			return _cachedLocalToParentMatrix;
		}
		private set => _cachedLocalToParentMatrix = value;
	}

	private Matrix4 _cachedParentToLocalMatrix;
	public Matrix4 ParentToLocalMatrix
	{
		get
		{
			if (_invalidLocalCache)
			{
				RecalculateLocalCache();
			}
		
			return _cachedParentToLocalMatrix;
		}
		private set => _cachedParentToLocalMatrix = value;
	}

	private Matrix4 _cachedLocalToWorldMatrix;
	public Matrix4 LocalToWorldMatrix
	{
		get
		{
			if (_invalidWorldCache)
			{
				RecalculateWorldCache();
			}
		
			return _cachedLocalToWorldMatrix;
		}
		private set => _cachedLocalToWorldMatrix = value;
	}

	private Matrix4 _cachedWorldToLocalMatrix;
	public Matrix4 WorldToLocalMatrix
	{
		get
		{
			if (_invalidWorldCache)
			{
				RecalculateWorldCache();
			}
		
			return _cachedWorldToLocalMatrix;
		}
		private set => _cachedWorldToLocalMatrix = value;
	}
	
	#endregion

	#region Direction
	
	public Vector2 LocalToWorldDirection(Vector2 localDir)
	{
		(float x, float y) = localDir;
		return (new Vector4(x, y, 0, 0) * LocalToWorldMatrix).Xy.Normalized();
	}

	public Vector2 WorldToLocalDirection(Vector2 worldDir)
	{
		(float x, float y) = worldDir;
		return (new Vector4(x, y, 0, 0) * WorldToLocalMatrix).Xy.Normalized();
	}

	public Vector2 LocalToParentDirection(Vector2 localDir)
	{
		(float x, float y) = localDir;
		return (new Vector4(x, y, 0, 0) * LocalToParentMatrix).Xy;
	}

	public Vector2 ParentToLocalDirection(Vector2 worldDir)
	{
		(float x, float y) = worldDir;
		return (new Vector4(x, y, 0, 0) * ParentToLocalMatrix).Xy;
	}
	
	#endregion
	
	private void InvalidateLocalCache()
	{
		if (_invalidLocalCache) return;
		_invalidLocalCache = true;
		InvalidateWorldCache();
	}

	private void RecalculateLocalCache()
	{
		_cachedLocalToParentMatrix = Matrix4.CreateRotationZ(-TKMathHelper.DegreesToRadians(_localRotation)) *
		                             Matrix4.CreateTranslation(_localPosition.X, _localPosition.Y, 0);

		_cachedParentToLocalMatrix = _cachedLocalToParentMatrix.Inverted();
		
		_invalidLocalCache = false;
	}

	private void InvalidateWorldCache()
	{
		if (_invalidWorldCache) return;
		_invalidWorldCache = true;

		for (int i = 0; i < Children.Count; i++)
		{
			Children[i].InvalidateWorldCache();
		}
	}

	private void RecalculateWorldCache()
	{
		Matrix4 parentLocalToWorld = _parent != null ? Parent.LocalToWorldMatrix: Matrix4.Identity;
		
		_cachedLocalToWorldMatrix = LocalToParentMatrix * parentLocalToWorld;
		_cachedWorldToLocalMatrix = _cachedLocalToWorldMatrix.Inverted();
		
		_cachedWorldPosition = _cachedLocalToWorldMatrix.ExtractTranslation().Xy;
		_cachedWorldRotation = CorrectRotation(TKMathHelper.RadiansToDegrees(_cachedLocalToWorldMatrix.ExtractRotation().ToEulerAngles().Z));
		
		_invalidWorldCache = false;
	}

	public Vector2 Right => LocalToWorldDirection(new Vector2(1, 0));

	public Vector2 Up => LocalToWorldDirection(new Vector2(0, 1));

	private static float CorrectRotation(float rotation)
	{
		return MathHelper.Modulo(rotation, 360);
	}
}