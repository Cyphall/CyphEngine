using CyphEngine.Components;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace TestGame;

public class TestEntityScript : AComponent
{
	private AudioPlayer _audioPlayer = null!;

	protected override void OnInit()
	{
		_audioPlayer = Entity.GetComponent<AudioPlayer>()!;
	}

	protected override void OnUpdate(float deltaTime)
	{
		Vector2 dir = Vector2.Zero;
		if (Engine.Window.KeyDown(Keys.W))
		{
			dir.Y += 1;
		}
		if (Engine.Window.KeyDown(Keys.S))
		{
			dir.Y -= 1;
		}
		if (Engine.Window.KeyDown(Keys.A))
		{
			dir.X -= 1;
		}
		if (Engine.Window.KeyDown(Keys.D))
		{
			dir.X += 1;
		}
		
		Transform.LocalPosition += dir * (300.0f * deltaTime);

		if (Window.KeyPressed(Keys.E))
		{
			_audioPlayer.Reset();
			_audioPlayer.Start();
		}
	}
}