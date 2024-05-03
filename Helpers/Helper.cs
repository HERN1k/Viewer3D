using Newtonsoft.Json;
using OpenTK.Mathematics;
using System.Drawing;
using System.Globalization;

namespace Viewer3D.Helpers
{
  public class Helper
  {
    public static async Task<Settings> GetSettings()
    {
      string fileSettings = $"{Environment.CurrentDirectory}/Settings/Settings.json";
      string jsonSettings = await File.ReadAllTextAsync(fileSettings);
      var settings = await Task.Run(() => JsonConvert.DeserializeObject<Settings>(jsonSettings)!);
      var settingsData = new Settings
      {
        ProgramName = settings?.ProgramName,
        ViewWidth = settings?.ViewWidth,
        ViewHeight = settings?.ViewHeight,
        UpdateFrequency = settings?.UpdateFrequency,
        Vsync = settings?.Vsync,
        WindowState = settings?.WindowState,

        ViewingDistance = settings?.ViewingDistance,
        PolygonMode = settings?.PolygonMode,

        BackgroundColor = settings?.BackgroundColor,
        LinesScale = settings?.LinesScale,
        CameraPosition = settings?.CameraPosition,

        ModelColor = settings?.ModelColor,
        ModelPosition = settings?.ModelPosition,
        ModelRotation = settings?.ModelRotation,
        ModelScale = settings?.ModelScale,
        ModelPath = settings?.ModelPath
      };
      return settingsData;
    }

    public static Vector3 HexToVector3(string hexColor)
    {
      Color color = ColorTranslator.FromHtml(hexColor);
      float red = (float)Math.Round((double)(color.R / 255.0f), 2);
      float green = (float)Math.Round((double)(color.G / 255.0f), 2);
      float blue = (float)Math.Round((double)(color.B / 255.0f), 2);
      var result = new Vector3(red, green, blue);
      return result;
    }

    public static Vector4 HexToVector4(string hexColor)
    {
      Color color = ColorTranslator.FromHtml(hexColor);
      float red = (float)Math.Round((double)(color.R / 255.0f), 2);
      float green = (float)Math.Round((double)(color.G / 255.0f), 2);
      float blue = (float)Math.Round((double)(color.B / 255.0f), 2);
      float alpha = (float)Math.Round((double)(color.A / 255.0f), 2);
      var result = new Vector4(red, green, blue, alpha);
      return result;
    }

    public static Color4 HexToColor4(string hexColor)
    {
      Color color = ColorTranslator.FromHtml(hexColor);
      return new Color4(color.R, color.G, color.B, color.A);
    }

    public static Vector3 StringToVector3(string position)
    {
      string[] path = position.Split(" ");
      float X = float.Parse(path[0], CultureInfo.InvariantCulture);
      float Y = float.Parse(path[1], CultureInfo.InvariantCulture);
      float Z = float.Parse(path[2], CultureInfo.InvariantCulture);

      return new Vector3(X, Y, Z);
    }

    public static string StringToPathModel(string modelPath)
    {
      string path = string.Empty;
      switch (modelPath)
      {
        case "/IronMan.obj":
          path = $"{Environment.CurrentDirectory}/IronMan.obj";
          break;
        case "/IronMan":
          path = $"{Environment.CurrentDirectory}/IronMan.obj";
          break;
        case "\\IronMan.obj":
          path = $"{Environment.CurrentDirectory}/IronMan.obj";
          break;
        case "\\IronMan":
          path = $"{Environment.CurrentDirectory}/IronMan.obj";
          break;
        case "IronMan.obj":
          path = $"{Environment.CurrentDirectory}/IronMan.obj";
          break;
        case "IronMan":
          path = $"{Environment.CurrentDirectory}/IronMan.obj";
          break;
        default:
          if (!File.Exists(modelPath))
            path = $"{Environment.CurrentDirectory}/IronMan.obj";
          else
            path = modelPath;
          break;
      }

      return path;
    }
  }
}
