using Viewer3D.ObjParser.Types;

namespace Viewer3D.ObjParser
{
  public class Parser
  {
    public List<Vertex> VertexList { get; set; }

    public List<Normals> NormalsList { get; set; }

    public List<TextureVertex> TextureList { get; set; }

    public List<Face> FaceList { get; set; }

    public Extent? Size { get; set; }

    public string? UseMtl { get; set; }

    public string? Mtl { get; set; }

    public Parser()
    {
      VertexList = new List<Vertex>();
      NormalsList = new List<Normals>();
      TextureList = new List<TextureVertex>();
      FaceList = new List<Face>();
    }

    public void LoadObj(string path)
    {
      LoadObj(File.ReadAllLines(path));
    }

    public void LoadObj(Stream data)
    {
      using (var reader = new StreamReader(data))
      {
        LoadObj(reader.ReadToEnd().Split(Environment.NewLine.ToCharArray()));
      }
    }

    public void LoadObj(IEnumerable<string> data)
    {
      foreach (var line in data)
      {
        processLine(line);
      }

      updateSize();
    }

    private void processLine(string line)
    {
      string[] parts = line.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

      if (parts.Length > 0)
      {
        switch (parts[0])
        {
          case "usemtl":
            UseMtl = parts[1];
            break;
          case "mtllib":
            Mtl = parts[1];
            break;
          case "v":
            Vertex v = new Vertex();
            v.LoadFromStringArray(parts);
            VertexList.Add(v);
            v.Index = VertexList.Count();
            break;
          case "vn":
            Normals vn = new Normals();
            vn.LoadFromStringArray(parts);
            NormalsList.Add(vn);
            vn.Index = NormalsList.Count();
            break;
          case "vt":
            TextureVertex vt = new TextureVertex();
            vt.LoadFromStringArray(parts);
            TextureList.Add(vt);
            vt.Index = TextureList.Count();
            break;
          case "f":
            Face f = new Face();
            f.LoadFromStringArray(parts);
            f.UseMtl = UseMtl;
            FaceList.Add(f);
            break;
        }
      }
    }

    private void updateSize()
    {
      if (VertexList.Count == 0)
      {
        Size = new Extent
        {
          XMax = 0,
          XMin = 0,
          YMax = 0,
          YMin = 0,
          ZMax = 0,
          ZMin = 0
        };
        return;
      }

      Size = new Extent
      {
        XMax = VertexList.Max(v => v.X),
        XMin = VertexList.Min(v => v.X),
        YMax = VertexList.Max(v => v.Y),
        YMin = VertexList.Min(v => v.Y),
        ZMax = VertexList.Max(v => v.Z),
        ZMin = VertexList.Min(v => v.Z)
      };
    }
  }
}
