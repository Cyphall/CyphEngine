using JetBrains.Annotations;

namespace CyphEngine.Entities;

[PublicAPI]
public interface IEntityPreset
{
	public void OnApply(Entity entity);

	private class EmptyEntityPreset : IEntityPreset
	{
		void IEntityPreset.OnApply(Entity entity) {}
	}
	public static IEntityPreset Empty => new EmptyEntityPreset();
}