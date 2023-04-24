using CyphEngine.Components;
using CyphEngine.Entities;
using OpenTK.Mathematics;

namespace TestGame;

public class TestEntityPreset : IEntityPreset
{
	void IEntityPreset.OnApply(Entity entity)
	{
		TestEntityScript script = entity.CreateComponent<TestEntityScript>();

		SpriteRenderer spriteRenderer = entity.CreateComponent<SpriteRenderer>();
		spriteRenderer.SetTexture(new Vector4(1, 0, 0, 1));
		spriteRenderer.Size = new Vector2(100, 50);

		AudioPlayer audioPlayer = entity.CreateComponent<AudioPlayer>();
		audioPlayer.LoadSound("iamtheprotectorofthissystem.wav");
	}
}