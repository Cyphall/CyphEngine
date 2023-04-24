using CyphEngine.Maths;
using CyphEngine.Rendering;
using CyphEngine.Scenes;
using JetBrains.Annotations;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace CyphEngine.UI;

[PublicAPI]
public class UIManager
{
	public Scene Scene { get; }
	public Engine Engine => Scene.Engine;
	public Window Window => Engine.Window;

	internal static UIManager Current { get; private set; } = null!;

	private UIRoot _root = null!;

	public Matrix4 Projection { get; }

	private CursorShape _previousCursor = CursorShape.Arrow;
	public CursorShape Cursor { get; set; } = CursorShape.Arrow;

	private bool _dirty;
	
	public void SetDirty()
	{
		_dirty = true;
	}

	internal UIManager(Scene scene)
	{
		Scene = scene;
		Current = this;
		
		SetUI(IUIPreset.Empty);

		(float width, float height) = Scene.Engine.Window.SimulatedSize;
		Projection = Matrix4.CreateOrthographicOffCenter(0, width, height, 0, -1000, 1000);
	}

	public void SetUI(IUIPreset preset)
	{
		Scene.Engine.Window.SetCursor(CursorShape.Arrow);
		
		_root = new UIRoot();
		
		preset.OnApply(_root);
	}

	internal void OnUpdate()
	{
		Cursor = CursorShape.Arrow;
		
		_root.Update();

		if (Cursor != _previousCursor)
		{
			Scene.Engine.Window.SetCursor(Cursor);
			_previousCursor = Cursor;
		}
	}

	internal void OnRender(Renderer renderer)
	{
		if (_dirty)
		{
			_root.Measure(Window.SimulatedSize);
			_root.Arrange(Rect.FromOriginSize(new Vector2(0, 0), Window.SimulatedSize));
			_dirty = false;
		}
		
		Matrix4 projection = Projection;
		_root.Render(renderer, ref projection);
	}
}