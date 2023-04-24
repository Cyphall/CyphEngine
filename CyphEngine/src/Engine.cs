using System.Diagnostics;
using System.Runtime.InteropServices;
using CyphEngine.Audio;
using CyphEngine.Rendering;
using CyphEngine.Resources;
using CyphEngine.Scenes;
using JetBrains.Annotations;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace CyphEngine;

[PublicAPI]
public sealed class Engine
{
	[DllImport("msvcrt.dll", CharSet = CharSet.Ansi)]
	private static extern int _putenv_s(string e, string v);

	private static void MessageCallback(
		DebugSource source,
		DebugType type,
		int id,
		DebugSeverity severity,
		int length,
		IntPtr message,
		IntPtr userParam)
	{
		string messageStr = Marshal.PtrToStringUTF8(message) ?? string.Empty;
		
		switch (severity)
		{
			case DebugSeverity.DebugSeverityHigh:
				Console.Error.WriteLine($"OpenGL error: {messageStr}");
				Debugger.Break();
				break;
			case DebugSeverity.DebugSeverityMedium:
				Console.WriteLine($"OpenGL warning: {messageStr}");
				break;
			case DebugSeverity.DebugSeverityLow:
				Console.WriteLine($"OpenGL info: {messageStr}");
				break;
		}
	}
		
	public static void Run(Vector2i? size, string title, IScenePreset initialScenePreset)
	{
		Engine engine = new Engine(size, title);
		engine.ScheduleSceneLoad(initialScenePreset);
		engine.Run();
		engine.Destroy();
	}
	
	public Window Window { get; }
	public Renderer Renderer { get; }

	private IScenePreset? _scheduledSceneLoadPreset;
	public Scene? Scene { get; private set; }

	internal ResourceManager ResourceManager { get; } = new ResourceManager();
	
	internal AudioManager AudioManager { get; }

	private DebugProc _debugProcCallback;

	private Engine(Vector2i? size, string title)
	{
		_putenv_s("ALSOFT_TRAP_ERROR", "true");

		GLFW.Init();
		
		Window = new Window(size, title);
		Window.MakeCurrent();
		
		GLFW.SwapInterval(1);
		
		GL.LoadBindings(new GLFWBindingsContext());
		
		GL.Enable(EnableCap.DebugOutput);
#if DEBUG
		GL.Enable(EnableCap.DebugOutputSynchronous);
#endif
		_debugProcCallback = MessageCallback;
		GL.DebugMessageCallback(_debugProcCallback, IntPtr.Zero);
		
		Renderer = new Renderer(this);

		AudioManager = new AudioManager();
	}

	public void ScheduleSceneLoad(IScenePreset preset)
	{
		_scheduledSceneLoadPreset = preset;
	}

	private void Run()
	{
		Stopwatch sw = new Stopwatch();

		while (!Window.ShouldClose)
		{
			float deltaTime = (float)sw.Elapsed.TotalSeconds;
			sw.Restart();
			
			if (_scheduledSceneLoadPreset != null)
			{
				Scene?.OnDestroyInternal();
				Scene = new Scene(this);
				_scheduledSceneLoadPreset.OnApply(Scene);
				_scheduledSceneLoadPreset = null;
			}
			
			GLFW.PollEvents();
			Window.HandleEvents();
			
			Scene?.OnUpdate(deltaTime);
			
			Scene?.OnRender(Renderer);
			Renderer.Render();
			
			Scene?.OnEndFrame();
			Window.SwapBuffers();
		}
	}

	private void Destroy()
	{
		Scene?.OnDestroyInternal();
		Scene = null;
		
		AudioManager.Dispose();
		Renderer.Dispose();
		ResourceManager.Dispose();
		Window.Dispose();
	}

	public void Quit()
	{
		Window.ShouldClose = true;
	}
}