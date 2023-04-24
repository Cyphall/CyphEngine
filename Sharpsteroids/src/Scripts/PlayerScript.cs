using CyphEngine.Components;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using Sharpsteroids.Presets;
using Sharpsteroids.Scripts.Weapons;
using MathHelper = CyphEngine.Helper.MathHelper;

namespace Sharpsteroids.Scripts;

public class PlayerScript : AComponent
{
	private Vector2 _velocity;

	public AudioPlayer ThrusterAudioPlayer { get; set; } = null!;
	
	public SpriteRenderer Thruster { get; set; } = null!;
	private float _thrusterCooldown;
	
	private WeaponType _currentWeapon;
	public WeaponType CurrentWeapon
	{
		get => _currentWeapon;
		set
		{
			_currentWeapon = value;

			for (int i = 0; i < Weapons.Count; i++)
			{
				Weapons[i].Enabled = Weapons[i].Type == _currentWeapon;
			}

			((MainSceneScript)Scene.MainScript!).UpdateUI(_currentWeapon);
		}
	}

	public List<AWeaponScript> Weapons { get; }= new List<AWeaponScript>();

	private BoxCollider _boxCollider = null!;

	protected override void OnInit()
	{
		_boxCollider = (BoxCollider)Entity.GetComponent<PhysicsCollider>()!.Collider!;
		
		Weapons.Add(Entity.GetComponent<BlasterScript>()!);
		Weapons.Add(Entity.GetComponent<ShotgunScript>()!);
		Weapons.Add(Entity.GetComponent<EnergyCannonScript>()!);
		Weapons.Add(Entity.GetComponent<SniperScript>()!);
		
		CurrentWeapon = WeaponType.Blaster;

		Thruster.Enabled = false;

		ThrusterAudioPlayer.Loop = true;
	}

	protected override void OnUpdate(float deltaTime)
	{
		Vector2 mouseDir = (Scene.MainCamera!.CursorWorldPos - Transform.LocalPosition).Normalized();
		Transform.LocalRotation = MathHelper.AngleFromVector(mouseDir);

		if (Window.MouseButtonPressed(MouseButton.Right))
		{
			_thrusterCooldown = 0;
			ThrusterAudioPlayer.Start();
		}
		
		if (Window.MouseButtonDown(MouseButton.Right))
		{
			_velocity += Transform.Up * deltaTime * 300;

			_thrusterCooldown -= deltaTime;
			if (_thrusterCooldown <= 0)
			{
				Thruster.Enabled = !Thruster.Enabled;
				_thrusterCooldown += 1.0f / 20.0f;
			}
		}
		else
		{
			Thruster.Enabled = false;
			ThrusterAudioPlayer.Stop();
		}

		if (Window.KeyPressed(Keys.D1))
		{
			CurrentWeapon = WeaponType.Blaster;
		}
		else if (Window.KeyPressed(Keys.D2))
		{
			CurrentWeapon = WeaponType.Shotgun;
		}
		else if (Window.KeyPressed(Keys.D3))
		{
			CurrentWeapon = WeaponType.EnergyCannon;
		}
		else if (Window.KeyPressed(Keys.D4))
		{
			CurrentWeapon = WeaponType.Sniper;
		}
		
		Transform.LocalPosition += _velocity * deltaTime;
		_velocity *= 1 - 0.6f * deltaTime;
		
		float radius = (_boxCollider.Size / 2).Length;
		Vector2 offsettedSceneHalfSize = Window.SimulatedSize / 2.0f + new Vector2(radius);

		Vector2 currentPosition = Transform.LocalPosition;
		Vector2 newPostion = currentPosition;

		if (currentPosition.X < -offsettedSceneHalfSize.X)
		{
			newPostion.X += offsettedSceneHalfSize.X * 2;
		}
		else if (currentPosition.X > offsettedSceneHalfSize.X)
		{
			newPostion.X -= offsettedSceneHalfSize.X * 2;
		}
		else if (currentPosition.Y < -offsettedSceneHalfSize.Y)
		{
			newPostion.Y += offsettedSceneHalfSize.Y * 2;
		}
		else if (currentPosition.Y > offsettedSceneHalfSize.Y)
		{
			newPostion.Y -= offsettedSceneHalfSize.Y * 2;
		}

		if (newPostion != currentPosition)
		{
			Transform.LocalPosition = newPostion;
		}
	}

	protected override void OnDestroy()
	{
		((MainSceneScript)Scene.MainScript!).PlayerDestroyed();

		Scene.CreateEntity(new ExplosionPreset("assets/sounds/explosion-small.wav", Transform.LocalPosition, _velocity, 12, 3), Scene.Root);
	}

	protected override void OnCollide(PhysicsCollider thisPhysicsCollider, PhysicsCollider otherPhysicsCollider)
	{
		Scene.DestroyEntity(Entity);
	}
}