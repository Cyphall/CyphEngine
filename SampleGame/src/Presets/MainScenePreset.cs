using CyphEngine.Entities;
using CyphEngine.Scenes;

namespace SampleGame;

public class MainScenePreset : IScenePreset
{
	void IScenePreset.OnApply(Scene scene)
	{
		Entity gameManager = scene.CreateEntity(new GameManagerPreset(), scene.Root);
		scene.CreateEntity(new PlayerPreset(), scene.Root, "Player");

		GameMainScript mainScript = scene.CreateMainScript<GameMainScript>();
		mainScript.GameManagerScript = gameManager.GetComponent<GameManagerScript>()!;
		
		scene.AllowCollisions("alien", "player");
		scene.AllowCollisions("alien", "playerbullet");
	}
}