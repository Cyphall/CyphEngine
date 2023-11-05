using CyphEngine.Components;
using CyphEngine.Entities;

namespace SampleGame;

public class AlienPreset : IEntityPreset
{
	private string _name;
	
	public AlienPreset(string name)
	{
		_name = name;
	}
	
	void IEntityPreset.OnApply(Entity entity)
	{
		SpriteRenderer spriteRenderer1 = entity.CreateComponent<SpriteRenderer>();
		spriteRenderer1.LoadTexture($"assets/sprites/{_name}_0.png");
		spriteRenderer1.ZOffset = 5;
		
		SpriteRenderer spriteRenderer2 = entity.CreateComponent<SpriteRenderer>();
		spriteRenderer2.LoadTexture($"assets/sprites/{_name}_1.png");
		spriteRenderer2.ZOffset = 5;
		
		SpriteAnimator spriteAnimator = entity.CreateComponent<SpriteAnimator>();
		spriteAnimator.RegisterSpriteRenderer(spriteRenderer1);
		spriteAnimator.RegisterSpriteRenderer(spriteRenderer2);
		
		PhysicsCollider physicsCollider = entity.CreateComponent<PhysicsCollider>();
		physicsCollider.Collider = new BoxCollider(spriteRenderer1.Size);
		physicsCollider.LayerName = "alien";
	}
}