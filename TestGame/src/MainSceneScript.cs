using CyphEngine.Scenes;
using CyphEngine.UI;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace TestGame;

public class MainSceneScript : ASceneMainScript
{
	private int _count = 1000;

	private int _spawnedCount = 0;

	protected override void OnUpdate(float deltaTime)
	{
		if (_spawnedCount > 0)
		{
			Console.WriteLine($"delta time: {deltaTime:F5}   sprites: {_spawnedCount}");
		}
		
		if (Window.KeyPressed(Keys.D1))
		{
			_count = 100;
		}
		else if (Window.KeyPressed(Keys.D2))
		{
			_count = 1000;
		}
		else if (Window.KeyPressed(Keys.D3))
		{
			_count = 10000;
		}
		else if (Window.KeyPressed(Keys.D4))
		{
			_count = 100000;
		}
		else if (Window.KeyPressed(Keys.D5))
		{
			_count = 1000000;
		}
		else if (Window.KeyPressed(Keys.D6))
		{
			_count = 10000000;
		}
		else if (Window.KeyPressed(Keys.D7))
		{
			_count = 100000000;
		}
		if (Window.KeyPressed(Keys.Space))
		{
			for (int i = 0; i < _count; i++)
			{
				Scene.CreateEntity(new SpriteEntityPreset(), Scene.Root);
				_spawnedCount++;
			}
		}
	}
}