using CyphEngine.Entities;
using CyphEngine.Scenes;
using CyphEngine.UI;
using OpenTK.Mathematics;
using Sharpsteroids.Presets;
using Sharpsteroids.Presets.UI;
using Sharpsteroids.Scripts.Weapons;
using MathHelper = CyphEngine.Helper.MathHelper;

namespace Sharpsteroids.Scripts;

public class MainSceneScript : ASceneMainScript
{
	public PlayerScript? Player { get; private set; }

	public List<Entity> Asteroids { get; } = new List<Entity>();

	private float _respawnCooldown;

	private List<Tuple<WeaponType, UIImage, UIImage>> _weaponIcons = new List<Tuple<WeaponType, UIImage, UIImage>>();

	private static readonly Dictionary<Tier, int> _asteroidsPerTier = new Dictionary<Tier, int>
	{
		{ Tier.Big, 5 },
		{ Tier.Medium, 5 },
		{ Tier.Small, 5 }
	};

	public void PlayerDestroyed()
	{
		Player = null;
		_weaponIcons.Clear();
	}

	public void UpdateUI(WeaponType weaponType)
	{
		for (int i = 0; i < _weaponIcons.Count; i++)
		{
			if (_weaponIcons[i].Item1 == weaponType)
			{
				_weaponIcons[i].Item2.Visibility = Visibility.Collapsed;
				_weaponIcons[i].Item3.Visibility = Visibility.Visible;
			}
			else
			{
				_weaponIcons[i].Item2.Visibility = Visibility.Visible;
				_weaponIcons[i].Item3.Visibility = Visibility.Collapsed;
			}
		}
	}

	private void CreateUI()
	{
		Scene.UIManager.SetUI(new HUDUIPreset(Player!, _weaponIcons));
	}

	protected override void OnUpdate(float deltaTime)
	{
		if (Asteroids.Count == 0 && Player != null)
		{
			Vector2 sceneHalfSize = Scene.Engine.Window.SimulatedSize / 2.0f;

			foreach (Tier tier in Enum.GetValues<Tier>())
			{
				int count = _asteroidsPerTier[tier];
				for (int i = 0; i < count; i++)
				{
					Vector2 randomPos;
					float playerDistance;

					do
					{
						randomPos = new Vector2
						{
							X = MathHelper.RandomFloat(-sceneHalfSize.X, sceneHalfSize.X) * 0.9f,
							Y = MathHelper.RandomFloat(-sceneHalfSize.Y, sceneHalfSize.Y) * 0.9f
						};
						playerDistance = (randomPos - Player.Transform.LocalPosition).Length;
					} while (playerDistance < 200);

					Entity asteroid = Scene.CreateEntity(new AsteroidPreset(tier, randomPos, MathHelper.RandomDirection()), Scene.Root, "Asteroid");
					Asteroids.Add(asteroid);

					asteroid.Transform.LocalPosition = randomPos;
				}
			}
		}
		
		if (Player == null)
		{
			_respawnCooldown -= deltaTime;
			if (_respawnCooldown <= 0)
			{
				Player = Scene.CreateEntity(new PlayerEntityPreset(), Scene.Root).GetComponent<PlayerScript>()!;
				_respawnCooldown = 3;
			}
		}

		if (Player != null && Player.Weapons.Count != _weaponIcons.Count)
		{
			CreateUI();
			UpdateUI(Player!.CurrentWeapon);
		}
	}
}