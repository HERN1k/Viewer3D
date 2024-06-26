﻿using System.Globalization;

namespace Viewer3D.ObjParser.Types
{
  public class TextureVertex
  {
    public int MinimumDataLength { get; } = 3;
    public string Prefix { get; } = "vt";

    public float X { get; set; }

    public float Y { get; set; }

    public int Index { get; set; }

    public void LoadFromStringArray(string[] data)
    {
      if (data.Length < MinimumDataLength)
        throw new ArgumentException("Input array must be of minimum length " + MinimumDataLength, "data");

      if (!data[0].ToLower().Equals(Prefix))
        throw new ArgumentException("Data prefix must be '" + Prefix + "'", "data");

      bool success;

      double x, y;

      success = double.TryParse(data[1], NumberStyles.Any, CultureInfo.InvariantCulture, out x);
      if (!success) throw new ArgumentException("Could not parse X parameter as double");

      success = double.TryParse(data[2], NumberStyles.Any, CultureInfo.InvariantCulture, out y);
      if (!success) throw new ArgumentException("Could not parse Y parameter as double");
      X = (float)x;
      Y = (float)y;
    }
  }
}
