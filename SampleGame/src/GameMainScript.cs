using CyphEngine.Entities;
using CyphEngine.Scenes;

namespace SampleGame;

public class GameMainScript : ASceneMainScript
{
	public Entity? PlayerBullet { get; set; }
	public GameManagerScript GameManagerScript { get; set; } = null!;
}