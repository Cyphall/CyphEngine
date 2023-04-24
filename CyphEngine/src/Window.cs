using JetBrains.Annotations;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using GLFWmonitor = OpenTK.Windowing.GraphicsLibraryFramework.Monitor;
using GLFWwindow = OpenTK.Windowing.GraphicsLibraryFramework.Window;
using GLFWvidmode = OpenTK.Windowing.GraphicsLibraryFramework.VideoMode;

namespace CyphEngine;

[PublicAPI]
public sealed unsafe class Window : IDisposable
{
	private GLFWwindow* _window;

	public Vector2 SimulatedSize { get; }

	private Dictionary<Keys, InputAction> _previousKeyState = new Dictionary<Keys, InputAction>();
	private Dictionary<Keys, InputAction> _currentKeyState = new Dictionary<Keys, InputAction>();

	private Dictionary<MouseButton, InputAction> _previousMouseButtonState = new Dictionary<MouseButton, InputAction>();
	private Dictionary<MouseButton, InputAction> _currentMouseButtonState = new Dictionary<MouseButton, InputAction>();

	private Dictionary<CursorShape, IntPtr> _cursors = new Dictionary<CursorShape, IntPtr>();

	public Vector2i FramebufferSize
	{
		get
		{
			Vector2i size;
			GLFW.GetFramebufferSize(_window, out size.X, out size.Y);
			return size;
		}
	}

	public float Scale
	{
		get
		{
			GLFW.GetWindowContentScale(_window, out float scale, out _);
			return scale;
		}
	}

	public Vector2 CursorPos
	{
		get
		{
			Vector2d pos;
			GLFW.GetCursorPos(_window, out pos.X, out pos.Y);
			return (Vector2)pos / Scale;
		}
	}

	public Window(Vector2i? size, string title)
	{
		GLFW.DefaultWindowHints();
		GLFW.WindowHint(WindowHintInt.ContextVersionMajor, 4);
		GLFW.WindowHint(WindowHintInt.ContextVersionMinor, 6);
		GLFW.WindowHint(WindowHintOpenGlProfile.OpenGlProfile, OpenGlProfile.Core);
		GLFW.WindowHint(WindowHintBool.Resizable, false);
		GLFW.WindowHint((WindowHintBool)139276, true); // GLFW_SCALE_TO_MONITOR
#if DEBUG
		GLFW.WindowHint(WindowHintBool.OpenGLDebugContext, true);
#endif

		if (size != null)
		{
			_window = GLFW.CreateWindow(size.Value.X, size.Value.Y, title, null, null);
		}
		else
		{
			GLFWmonitor* monitor = GLFW.GetPrimaryMonitor();
			VideoMode* videoMode = GLFW.GetVideoMode(monitor);

			_window = GLFW.CreateWindow(videoMode->Width, videoMode->Height, title, monitor, null);
		}
		
		CenterWindow();

		SimulatedSize = (Vector2)FramebufferSize / Scale;
		
		foreach (Keys key in Enum.GetValues<Keys>())
		{
			_previousKeyState[key] = InputAction.Release;
			_currentKeyState[key] = InputAction.Release;
		}
		
		foreach (MouseButton mouseButton in Enum.GetValues<MouseButton>())
		{
			_previousMouseButtonState[mouseButton] = InputAction.Release;
			_currentMouseButtonState[mouseButton] = InputAction.Release;
		}
		
		_cursors[CursorShape.Arrow] = IntPtr.Zero;
	}

	private void CenterWindow()
	{
		GLFWmonitor* monitor = GLFW.GetPrimaryMonitor();
		GLFWvidmode* vidmode = GLFW.GetVideoMode(monitor);

		Vector2i contentSize;
		GLFW.GetWindowSize(_window, out contentSize.X, out contentSize.Y);

		GLFW.GetWindowFrameSize(_window, out int leftBorderSize, out int topBorderSize, out int rightBorderSize, out int bottomBorderSize);

		(int windowWidth, int windowHeight) = contentSize;
		windowWidth += leftBorderSize + rightBorderSize;
		windowHeight += topBorderSize + bottomBorderSize;
		
		GLFW.SetWindowPos(_window, (vidmode->Width - windowWidth) / 2, (vidmode->Height - windowHeight) / 2);
	}

	public void MakeCurrent()
	{
		GLFW.MakeContextCurrent(_window);
	}

	public void Dispose()
	{
		GLFW.DestroyWindow(_window);
	}

	public bool ShouldClose
	{
		get => GLFW.WindowShouldClose(_window);
		internal set => GLFW.SetWindowShouldClose(_window, value);
	}

	public void SwapBuffers()
	{
		GLFW.SwapBuffers(_window);
	}

	public void SetCursor(CursorShape cursor)
	{
		if (!_cursors.TryGetValue(cursor, out IntPtr cursorPtr))
		{
			cursorPtr = (IntPtr)GLFW.CreateStandardCursor(cursor);
			_cursors[cursor] = cursorPtr;
		}
		
		GLFW.SetCursor(_window, (Cursor*)_cursors[cursor]);
	}

	public bool KeyDown(Keys key)
	{
		return _currentKeyState[key] == InputAction.Press;
	}

	public bool KeyPressed(Keys key)
	{
		return _previousKeyState[key] == InputAction.Release && _currentKeyState[key] == InputAction.Press;
	}

	public bool KeyReleased(Keys key)
	{
		return _previousKeyState[key] == InputAction.Press && _currentKeyState[key] == InputAction.Release;
	}

	public bool MouseButtonDown(MouseButton mouseButton)
	{
		return _currentMouseButtonState[mouseButton] == InputAction.Press;
	}

	public bool MouseButtonPressed(MouseButton mouseButton)
	{
		return _previousMouseButtonState[mouseButton] == InputAction.Release && _currentMouseButtonState[mouseButton] == InputAction.Press;
	}

	public bool MouseButtonReleased(MouseButton mouseButton)
	{
		return _previousMouseButtonState[mouseButton] == InputAction.Press && _currentMouseButtonState[mouseButton] == InputAction.Release;
	}

	internal void HandleEvents()
	{
		foreach (Keys key in Enum.GetValues<Keys>())
		{
			_previousKeyState[key] = _currentKeyState[key];
		}
		foreach (Keys key in Enum.GetValues<Keys>())
		{
			_currentKeyState[key] = GLFW.GetKey(_window, key);
		}

		foreach (MouseButton mouseButton in Enum.GetValues<MouseButton>())
		{
			_previousMouseButtonState[mouseButton] = _currentMouseButtonState[mouseButton];
		}
		foreach (MouseButton mouseButton in Enum.GetValues<MouseButton>())
		{
			_currentMouseButtonState[mouseButton] = GLFW.GetMouseButton(_window, mouseButton);
		}
	}
}