using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Graphics.OpenGL4;
using Viewer3D.Helpers;
using Viewer3D.Window;

namespace Viewer3D
{
  internal class Program
  {
    public static Settings? Settings { get; set; }

    static void Main(string[] args)
    {
      Settings = Helper.GetSettings().Result;
      var program = new MainProgram();

      program.Start();
    }
  }

  public class MainProgram
  {
    private static MainWindow? _window { get; set; }

    private static Settings? _settings { get; set; } 

    private static string _name { get; set; } = "Error deserialize settings.json!";

    private static Vector2i _sizeWindow { get; set; } = new Vector2i(800, 600);

    private static string _title { get; set; } = "Error deserialize settings.json!";

    public static string WindowTitle
    {
      get => _title;
      set
      {
        _title = $"{_name} [{_sizeWindow.X}x{_sizeWindow.Y}] FPS:{value}";
        if (_window != null)
          _window.Title = _title;
      }
    }

    private static VSyncMode _syncMode { get; set; } = VSyncMode.On;

    private static WindowState _windowState { get; set; } = WindowState.Normal;

    private static int _updateFrequency { get; set; } = 60;

    public MainProgram()
    {
      SetSettings();
    }

    public void SetSettings()
    {
      _settings = Program.Settings;
      if (_settings == null) return;
      _name = _settings.ProgramName ?? _name;
      _title = $"{_name} Loading...";
      if (_settings.ViewWidth != null && _settings.ViewHeight != null)
        _sizeWindow = new Vector2i((int)_settings.ViewWidth, (int)_settings.ViewHeight);
      _syncMode = _settings.Vsync ?? _syncMode;
      _windowState = _settings.WindowState ?? _windowState;
      _updateFrequency = _settings.UpdateFrequency ?? _updateFrequency;
    }

    public void Start()
    {
      var nativeWindowSettings = new NativeWindowSettings()
      {
        ClientSize = _sizeWindow,
        Title = _title,
        Vsync = _syncMode,
        WindowState = _windowState,
        // This is needed to run on macos
        Flags = ContextFlags.ForwardCompatible
      };
      
      var gameWindowSettings = new GameWindowSettings()
      {
        UpdateFrequency = _updateFrequency
      };

      using (_window = new MainWindow(gameWindowSettings, nativeWindowSettings))
      {
        _window.Run();
      }
    }
  }

  public class Settings()
  {
    public string? ProgramName { get; set; }
    public int? ViewHeight { get; set; }
    public int? ViewWidth { get; set; }
    public int? UpdateFrequency { get; set; }
    public VSyncMode? Vsync { get; set; }
    public WindowState? WindowState { get; set; }

    public float? ViewingDistance { get; set; }
    public PolygonMode? PolygonMode { get; set; }

    public string? BackgroundColor { get; set; }
    public float? LinesScale { get; set; }
    public string? CameraPosition { get; set; }

    public string? ModelColor { get; set; }
    public string? ModelPosition { get; set; }
    public float? ModelRotation { get; set; }
    public float? ModelScale { get; set; }
    public string? ModelPath { get; set; }
  }
}
