using CyphEngine.Components;
using CyphEngine.Entities;

namespace SampleGame;

public class GameManagerPreset : IEntityPreset
{
	void IEntityPreset.OnApply(Entity entity)
	{
		entity.CreateComponent<Camera>();
		entity.CreateComponent<AlienManagerScript>();
		entity.CreateComponent<GameManagerScript>();
	}
}