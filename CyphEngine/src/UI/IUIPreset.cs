using JetBrains.Annotations;

namespace CyphEngine.UI;

[PublicAPI]
public interface IUIPreset
{
	public void OnApply(UIRoot root);

	private class EmptyUIPreset : IUIPreset
	{
		void IUIPreset.OnApply(UIRoot root) {}
	}
	public static IUIPreset Empty => new EmptyUIPreset();
}