using CyphEngine.Components;
using CyphEngine.Entities;
using OpenTK.Mathematics;

namespace SampleGame;

public class AlienManagerScript : AComponent
{
	private class AlienData
	{
		public Transform Transform;
		public SpriteAnimator Animator;

		public AlienData(Transform transform, SpriteAnimator animator)
		{
			Transform = transform;
			Animator = animator;
		}
	}

	private enum Direction
	{
		Left,
		Right
	}
	
	private int _scale = 3;

	private AlienData[] _aliens = new AlienData[11 * 5];

	private int _nextUpdate;

	private float _moveCooldown = 1.0f / 60.0f;
	private float _currentMoveCooldown;

	private const int PADDING = 24;
	
	private bool _goDown;
	private Direction _direction = Direction.Right;
	
	protected override void OnInit()
	{
		Vector2 topLeft = Window.SimulatedSize * new Vector2(-0.5f, 0.5f);

		string[] names =
		{
			"alien_0",
			"alien_1",
			"alien_1",
			"alien_2",
			"alien_2"
		};

		int i = 0;
		for (int y = 4; y >= 0; y--)
		{
			for (int x = 0; x < 11; x++)
			{
				Entity alien = Scene.CreateEntity(new AlienPreset(names[y]), Scene.Root, $"Alien {i}");
				_aliens[i] = new AlienData(alien.Transform, alien.GetComponent<SpriteAnimator>()!);
				
				int currentX = (PADDING + x * 16) * _scale;
				int currentY = (PADDING + y * 16) * _scale;
				_aliens[i].Transform.LocalPosition = topLeft + new Vector2(currentX, -currentY);

				i++;
			}
		}
	}

	private void MoveNext()
	{
		_nextUpdate = (_nextUpdate + 1) % (11 * 5);

		if (_nextUpdate == 0)
		{
			if (_goDown)
			{
				_goDown = false;
			}
			
			switch (_direction)
			{
				case Direction.Right:
				{
					AlienData? rightmostAlien = null;
					for (int i = 0; i < _aliens.Length; i++)
					{
						if (!_aliens[i].Transform.Owner.IsValid)
							continue;

						if (rightmostAlien == null || rightmostAlien.Transform.WorldPosition.X < _aliens[i].Transform.WorldPosition.X)
						{
							rightmostAlien = _aliens[i];
						}
					}

					if (rightmostAlien == null)
						return;

					if (!Scene.MainCamera!.IsInView(rightmostAlien.Transform.WorldPosition + new Vector2(PADDING, 0) * _scale + Displacement))
					{
						_goDown = true;
						_direction = Direction.Left;
					}
					break;
				}
				case Direction.Left:
				{
					AlienData? leftmostAlien = null;
					for (int i = 0; i < _aliens.Length; i++)
					{
						if (!_aliens[i].Transform.Owner.IsValid)
							continue;

						if (leftmostAlien == null || leftmostAlien.Transform.WorldPosition.X > _aliens[i].Transform.WorldPosition.X)
						{
							leftmostAlien = _aliens[i];
						}
					}

					if (leftmostAlien == null)
						return;

					if (!Scene.MainCamera!.IsInView(leftmostAlien.Transform.WorldPosition + new Vector2(-PADDING, 0) * _scale + Displacement))
					{
						_goDown = true;
						_direction = Direction.Right;
					}
					break;
				}
			}
		}
	}

	protected override void OnUpdate(float deltaTime)
	{
		_currentMoveCooldown -= deltaTime;
		
		if (_currentMoveCooldown > 0)
		{
			return;
		}

		_currentMoveCooldown += _moveCooldown;

		int i = 0;
		while (!_aliens[_nextUpdate].Transform.Owner.IsValid)
		{
			MoveNext();
			i++;

			if (i == _aliens.Length)
			{
				return;
			}
		}

		_aliens[_nextUpdate].Transform.LocalPosition += Displacement;
		_aliens[_nextUpdate].Animator.NextSprite();
		MoveNext();
	}

	private Vector2 Displacement
	{
		get
		{
			if (_goDown)
			{
				return new Vector2(0, -16) * _scale;
			}

			return _direction switch
			{
				Direction.Left => new Vector2(-2, 0) * _scale,
				Direction.Right => new Vector2(2, 0) * _scale,
				_ => throw new InvalidOperationException()
			};
		}
	}
}