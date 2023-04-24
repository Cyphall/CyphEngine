using CyphEngine.Components;
using CyphEngine.Entities;

namespace TestGame;

public class SpriteEntityPreset : IEntityPreset
{
	public void OnApply(Entity entity)
	{
		SpriteRenderer spriteRenderer = entity.CreateComponent<SpriteRenderer>();
		spriteRenderer.LoadTexture("test button.png");
	}
}