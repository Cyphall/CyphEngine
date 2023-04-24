using CyphEngine.UI;
using OpenTK.Mathematics;
using Sharpsteroids.Scripts;
using Sharpsteroids.Scripts.Weapons;

namespace Sharpsteroids.Presets.UI;

public class HUDUIPreset : IUIPreset
{
	private PlayerScript _playerScript;
	private List<Tuple<WeaponType, UIImage, UIImage>> _weaponIcons;

	public HUDUIPreset(PlayerScript playerScript, List<Tuple<WeaponType, UIImage, UIImage>> weaponIcons)
	{
		_playerScript = playerScript;
		_weaponIcons = weaponIcons;
	}

	public void OnApply(UIRoot root)
	{
		UICanvas canvas = new UICanvas();
		root.Child = canvas;
		
		UIStackPanel stackPanel = new UIStackPanel();
		stackPanel.Direction = Direction.LeftToRight;
		canvas.AddChild(stackPanel);
		
		for (int i = 0; i < _playerScript.Weapons.Count; i++)
		{
			AWeaponScript weapon = _playerScript.Weapons[i];

			UIGrid grid = new UIGrid();
			grid.Margin = 15;
			stackPanel.AddChild(grid);
			
			UIImage unselected = new UIImage();
			unselected.LoadTexture($"assets/sprites/GUI sprites/{weapon.Name}.png");
			grid.AddChild(unselected);
			
			UIImage selected = new UIImage();
			selected.LoadTexture($"assets/sprites/GUI sprites/{weapon.Name}_s.png");
			grid.AddChild(selected);
			
			_weaponIcons.Add(new Tuple<WeaponType, UIImage, UIImage>(weapon.Type, unselected, selected));
		}
	}
}