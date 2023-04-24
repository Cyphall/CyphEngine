using JetBrains.Annotations;

namespace CyphEngine.Scenes;

[PublicAPI]
public interface IScenePreset
{
	public void OnApply(Scene scene);

	private class EmptyScenePreset : IScenePreset
	{
		void IScenePreset.OnApply(Scene scene) {}
	}
	public static IScenePreset Empty => new EmptyScenePreset();
}