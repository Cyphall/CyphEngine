using CyphEngine.Components;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace SampleGame;

public class PlayerScript : AComponent
{
	protected override void OnUpdate(float deltaTime)
	{
		float dir = 0;
		if (Window.KeyDown(Keys.A) && Entity.Transform.WorldPosition.X > -300)
		{
			dir += -1;
		}
		if (Window.KeyDown(Keys.D) && Entity.Transform.WorldPosition.X < 300)
		{
			dir += 1;
		}
		
		Transform.LocalPosition += new Vector2(dir * 300.0f * deltaTime, 0);

		GameMainScript mainScript = (Scene.MainScript as GameMainScript)!;
		
		if (mainScript.PlayerBullet == null && Engine.Window.KeyDown(Keys.Space))
		{
			mainScript.PlayerBullet = Scene.CreateEntity(new BulletPreset(Transform.LocalPosition), Scene.Root, "Player bullet");
		}
	}

	protected override void OnCollide(PhysicsCollider thisCollider, PhysicsCollider otherCollider)
	{
		Engine.ScheduleSceneLoad(new MenuScenePreset());
	}
}