using JetBrains.Annotations;

namespace CyphEngine.Components;

[PublicAPI]
public class SpriteAnimator : AComponent
{
	private List<SpriteRenderer> _spriteRenderers = new List<SpriteRenderer>();

	private float _currentCooldown;
	private float _interval;
	public float Interval
	{
		get => _interval;
		set
		{
			float previousInterval = _interval;
			_interval = value;
			
			_currentCooldown += _interval - previousInterval;
		}
	}

	private int _currentSprite;
	public int CurrentSprite
	{
		get => _currentSprite;
		set
		{
			if (Enabled)
				_spriteRenderers[CurrentSprite].Enabled = false;
			
			_currentSprite = value % _spriteRenderers.Count;
			
			if (Enabled)
				_spriteRenderers[CurrentSprite].Enabled = true;
		} 
	}

	public void RegisterSpriteRenderer(SpriteRenderer spriteRenderer)
	{
		_spriteRenderers.Add(spriteRenderer);

		if (_spriteRenderers[CurrentSprite] == spriteRenderer)
		{
			spriteRenderer.Enabled = true;
		}
		else
		{
			spriteRenderer.Enabled = false;
		}
	}

	protected internal override void OnUpdate(float deltaTime)
	{
		if (Interval > 0)
		{
			_currentCooldown -= deltaTime;
		
			if (_currentCooldown > 0)
			{
				return;
			}

			_currentCooldown += Interval;

			NextSprite();
		}
	}
	
	public void NextSprite()
	{
		CurrentSprite++;
	}

	public void Reset()
	{
		CurrentSprite = 0;
		_currentCooldown = Interval;
	}

	protected internal override void OnDisable()
	{
		_spriteRenderers[CurrentSprite].Enabled = false;
	}

	protected internal override void OnEnable()
	{
		_spriteRenderers[CurrentSprite].Enabled = true;
	}
}