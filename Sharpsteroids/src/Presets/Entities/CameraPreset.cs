using CyphEngine.Components;
using CyphEngine.Entities;

namespace Sharpsteroids.Presets;

public class CameraPreset : IEntityPreset
{
	void IEntityPreset.OnApply(Entity entity)
	{
		entity.CreateComponent<Camera>();
	}
}