using CyphEngine.Components;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Sharpsteroids.Scripts.Weapons;

public enum WeaponType
{
	Blaster,
	Sniper,
	Shotgun,
	EnergyCannon
}

public abstract class AWeaponScript : AComponent
{
	private float _currentCooldown;
	public float Cooldown { get; protected set; }
	public WeaponType Type { get; protected set; }
	public string Name { get; protected set; } = null!;

	public AudioPlayer AudioPlayer { get; set; } = null!;

	protected override void OnUpdate(float deltaTime)
	{
		_currentCooldown -= deltaTime;

		if (_currentCooldown <= 0 && Window.MouseButtonDown(MouseButton.Left))
		{
			_currentCooldown = Cooldown;
			AudioPlayer.Start();
			Fire();
		}
	}

	protected abstract void Fire();
}