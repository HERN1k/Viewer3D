using System.Globalization;

namespace Viewer3D.ObjParser.Types
{
  public class Face
  {
    public int MinimumDataLength { get; } = 4;

    public string Prefix { get; } = "f";

    public string UseMtl { get; set; } = "";

    public int[] VertexIndexList { get; set; } = [];

    public int[] TextureVertexIndexList { get; set; } = [];

    public int[] NormalsVertexIndexList { get; set; } = [];

    public void LoadFromStringArray(string[] data)
    {
      if (data.Length < MinimumDataLength)
        throw new ArgumentException("Input array must be of minimum length " + MinimumDataLength, "data");

      if (!data[0].ToLower().Equals(Prefix))
        throw new ArgumentException("Data prefix must be '" + Prefix + "'", "data");

      int vcount = data.Count() - 1;
      VertexIndexList = new int[vcount];
      TextureVertexIndexList = new int[vcount];
      NormalsVertexIndexList = new int[vcount];

      bool success;

      for (int i = 0; i < vcount; i++)
      {
        string[] parts = data[i + 1].Split('/');

        int vindex;
        success = int.TryParse(parts[0], NumberStyles.Any, CultureInfo.InvariantCulture, out vindex);
        if (!success) throw new ArgumentException("Could not parse parameter as int");
        VertexIndexList[i] = vindex;

        if (parts.Count() > 1)
        {
          success = int.TryParse(parts[1], NumberStyles.Any, CultureInfo.InvariantCulture, out vindex);
          if (success)
          {
            TextureVertexIndexList[i] = vindex;
          }
        }

        if (parts.Count() > 2)
        {
          success = int.TryParse(parts[2], NumberStyles.Any, CultureInfo.InvariantCulture, out vindex);
          if (success)
          {
            NormalsVertexIndexList[i] = vindex;
          }
        }
      }
    }
  }
}
