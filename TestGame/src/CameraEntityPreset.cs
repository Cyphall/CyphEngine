using CyphEngine.Components;
using CyphEngine.Entities;

namespace TestGame;

public class CameraEntityPreset : IEntityPreset
{
	void IEntityPreset.OnApply(Entity entity)
	{
		entity.CreateComponent<Camera>();
	}
}