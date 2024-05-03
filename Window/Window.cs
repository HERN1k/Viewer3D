using OpenTK.Windowing.Desktop;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using Viewer3D.Shaders;
using Viewer3D.ObjParser;
using Viewer3D.Camera;
using OpenTK.Mathematics;
using Viewer3D.Helpers;

namespace Viewer3D.Window
{
  public class MainWindow : GameWindow
  {
    private Settings? _settings { get; set; }

    private int _vboLines { get; set; }

    private int _vboLamp { get; set; }

    private int _vboModel { get; set; }

    private int _vaoLines { get; set; }

    private int _vaoLamp { get; set; }

    private int _vaoModel { get; set; }

    private float[] _verticesLines { get; set; }

    private float[] _verticesModel { get; set; }

    private uint[] _indicesModel { get; set; }

    private float[] _verticesLamp { get; set; }

    private uint[] _indicesLamp { get; set; }

    private Shader _linesShader { get; set; }

    private Shader _lampShader { get; set; }

    private Shader _lightingShader { get; set; }

    private MainCamera _camera { get; set; }

    private bool _firstMove { get; set; } = true;

    private Vector2 _lastPos { get; set; }

    private Parser parserModel { get; set; }

    private Parser parserLamp { get; set; }

    private Parser parserLines { get; set; }

    private struct ParseData
    {
      public float[] vertices;
      public uint[] indices;
    }

    private double _time { get; set; }

    private float _frameTime { get; set; } = 0.0f;

    private int _fps { get; set; } = 0;

    private float _cameraSpeed { get; set; } = 10.0f;

    private float _viewingDistance { get; set; } = 1000.0f;

    private PolygonMode _polygonMode { get; set; } = PolygonMode.Fill;

    private Color4 _bgColor { get; set; } = new Color4(0.18f, 0.18f, 0.18f, 1.0f);

    private float _linesScale { get; set; } = 0.0f;

    private Vector3 _lightPosition { get; set; } = new Vector3(0.0f);

    private Vector3 _cameraPosition { get; set; } = new Vector3(0.0f, 7.0f, 10.0f);

    private float _lightScale { get; set; } = 0.0f;

    private Vector3 _modelColor { get; set; } = new Vector3(0.95f);

    private Vector3 _modelPosition { get; set; } = new Vector3(0.0f);

    private float _modelRotation { get; set; } = 20.0f;

    private float _modelScale { get; set; } = 0.05f;

    private string _pathLines { get; } = $"{Environment.CurrentDirectory}/Lines.obj";

    private string _pathLamp { get; } = $"{Environment.CurrentDirectory}/Light.obj";

    private string _pathModel { get; set; } = $"{Environment.CurrentDirectory}/IronMan.obj";

    public MainWindow(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings)
            : base(gameWindowSettings, nativeWindowSettings)
    {
      if (Program.Settings != null)
      {
        _settings = Program.Settings;
        SetSettings();
      }

      _lightingShader = new Shader("Shaders/shader.vert", "Shaders/lighting.frag");
      _lampShader = new Shader("Shaders/shader.vert", "Shaders/shader.frag");
      _linesShader = new Shader("Shaders/lines.vert", "Shaders/lines.frag");

      parserLines = new Parser();
      var dataLines = ParseFileLines();
      _verticesLines = dataLines;

      parserLamp = new Parser();
      var dataLamp = ParseFileLamp();
      _verticesLamp = dataLamp.vertices;
      _indicesLamp = dataLamp.indices;

      parserModel = new Parser();
      var dataModel = ParseFileModel();
      _verticesModel = dataModel.vertices;
      _indicesModel = dataModel.indices;

      _camera = new MainCamera(_cameraPosition, Size.X / (float)Size.Y, _viewingDistance);
    }

    protected override void OnLoad()
    {
      base.OnLoad();

      GL.ClearColor(_bgColor);

      GL.Enable(EnableCap.DepthTest);
      GL.Enable(EnableCap.CullFace);
      GL.Enable(EnableCap.Multisample);

      GL.PolygonMode(MaterialFace.FrontAndBack, _polygonMode);
      CursorState = CursorState.Grabbed;

      {
        _vaoLines = GL.GenVertexArray();
        GL.BindVertexArray(_vaoLines);

        _vboLines = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ArrayBuffer, _vboLines);
        GL.BufferData(BufferTarget.ArrayBuffer, _verticesLines.Length * sizeof(float), _verticesLines, BufferUsageHint.StaticDraw);

        int vertexLocation = _linesShader.GetAttribLocation("aPosition");
        GL.EnableVertexAttribArray(vertexLocation);
        GL.VertexAttribPointer(vertexLocation, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 0);

        int colorLocation = _linesShader.GetAttribLocation("aColor");
        GL.EnableVertexAttribArray(colorLocation);
        GL.VertexAttribPointer(colorLocation, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 3 * sizeof(float));
      }

      {
        _vaoLamp = GL.GenVertexArray();
        GL.BindVertexArray(_vaoLamp);

        _vboLamp = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ArrayBuffer, _vboLamp);
        GL.BufferData(BufferTarget.ArrayBuffer, _verticesLamp.Length * sizeof(float), _verticesLamp, BufferUsageHint.StaticDraw);
        
        int vertexLocation = _lampShader.GetAttribLocation("aPosition");
        GL.EnableVertexAttribArray(vertexLocation);
        GL.VertexAttribPointer(vertexLocation, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
      }

      {
        _vaoModel = GL.GenVertexArray();
        GL.BindVertexArray(_vaoModel);

        _vboModel = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ArrayBuffer, _vboModel);
        GL.BufferData(BufferTarget.ArrayBuffer, _verticesModel.Length * sizeof(float), _verticesModel, BufferUsageHint.StaticDraw);

        int vertexLocation = _lightingShader.GetAttribLocation("aPosition");
        GL.EnableVertexAttribArray(vertexLocation);
        GL.VertexAttribPointer(vertexLocation, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 0);

        int normalLocation = _lightingShader.GetAttribLocation("aNormal");
        GL.EnableVertexAttribArray(normalLocation);
        GL.VertexAttribPointer(normalLocation, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 3 * sizeof(float));
      }
    }

    protected override void OnRenderFrame(FrameEventArgs e)
    {
      base.OnRenderFrame(e);

      _frameTime += (float)e.Time;
      _fps++;
      if (_frameTime > 1.0f)
      {
        MainProgram.WindowTitle = $"{_fps}";
        _frameTime = 0.0f;
        _fps = 0;
      }

      _time += _modelRotation * e.Time;

      GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

      _lightingShader.SetVector3("objectColor", _modelColor);
      _lightingShader.SetVector3("lightPos", _lightPosition);
      _lightingShader.SetVector3("viewPos", _camera.Position);

      // ----------------------------------------------------------------------------

      GL.BindVertexArray(_vaoLines);

      var linesMatrix = Matrix4.Identity;
      linesMatrix *= Matrix4.CreateScale(_linesScale);

      _linesShader.SetMatrix4("model", linesMatrix);
      _linesShader.SetMatrix4("view", _camera.GetViewMatrix());
      _linesShader.SetMatrix4("projection", _camera.GetProjectionMatrix());

      _linesShader.Use();

      GL.Enable(EnableCap.Blend);
      GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
      GL.DrawArrays(PrimitiveType.Lines, 0, 6);
      GL.Disable(EnableCap.Blend);

      // ----------------------------------------------------------------------------

      GL.BindVertexArray(_vaoLamp);

      var lampMatrix = Matrix4.Identity;
      lampMatrix *= Matrix4.CreateScale(_lightScale);
      lampMatrix *= Matrix4.CreateTranslation(_lightPosition);

      _lampShader.SetMatrix4("model", lampMatrix);
      _lampShader.SetMatrix4("view", _camera.GetViewMatrix());
      _lampShader.SetMatrix4("projection", _camera.GetProjectionMatrix());

      _lampShader.Use();

      GL.DrawArrays(PrimitiveType.Triangles, 0, _indicesLamp.Length);

      // ----------------------------------------------------------------------------

      GL.BindVertexArray(_vaoModel);

      var modelMatrix = Matrix4.Identity;
      modelMatrix *= Matrix4.CreateRotationY((float)MathHelper.DegreesToRadians(_time));
      modelMatrix *= Matrix4.CreateTranslation(_modelPosition);
      modelMatrix *= Matrix4.CreateScale(_modelScale);

      _lightingShader.SetMatrix4("model", modelMatrix);
      _lightingShader.SetMatrix4("view", _camera.GetViewMatrix());
      _lightingShader.SetMatrix4("projection", _camera.GetProjectionMatrix());

      _lightingShader.Use();

      GL.DrawArrays(PrimitiveType.Triangles, 0, _indicesModel.Length);

      // ----------------------------------------------------------------------------

      SwapBuffers();
    }

    protected override void OnUpdateFrame(FrameEventArgs e)
    {
      base.OnUpdateFrame(e);

      var input = KeyboardState;
      var mouse = MouseState;
      const float sensitivity = 0.1f;

      if (!IsFocused) return;

      if (input.IsKeyDown(Keys.Escape))
        Close();

      if (input.IsKeyPressed(Keys.D1))
        _cameraSpeed = 2.0f;

      if (input.IsKeyPressed(Keys.D2))
        _cameraSpeed = 4.0f;

      if (input.IsKeyPressed(Keys.D3))
        _cameraSpeed = 8.0f;

      if (input.IsKeyPressed(Keys.D4))
        _cameraSpeed = 16.0f;

      if (input.IsKeyPressed(Keys.D5))
        _cameraSpeed = 32.0f;

      if (input.IsKeyPressed(Keys.D6))
        _cameraSpeed = 64.0f;

      if (input.IsKeyPressed(Keys.D7))
        _cameraSpeed = 128.0f;

      if (input.IsKeyPressed(Keys.D8))
        _cameraSpeed = 256.0f;

      if (input.IsKeyPressed(Keys.D9))
        _cameraSpeed = 512.0f;

      if (input.IsKeyPressed(Keys.D0))
        _cameraSpeed = 1024.0f;

      if (input.IsKeyDown(Keys.W))
      {
        _camera.Position += _camera.Front * _cameraSpeed * (float)e.Time; // Forward
        _lightPosition = new Vector3(_camera.Position.X, _camera.Position.Y, _camera.Position.Z);
      }
      if (input.IsKeyDown(Keys.S))
      {
        _camera.Position -= _camera.Front * _cameraSpeed * (float)e.Time; // Backwards
        _lightPosition = new Vector3(_camera.Position.X, _camera.Position.Y, _camera.Position.Z);
      }
      if (input.IsKeyDown(Keys.A))
      {
        _camera.Position -= _camera.Right * _cameraSpeed * (float)e.Time; // Left
        _lightPosition = new Vector3(_camera.Position.X, _camera.Position.Y, _camera.Position.Z);
      }
      if (input.IsKeyDown(Keys.D))
      {
        _camera.Position += _camera.Right * _cameraSpeed * (float)e.Time; // Right
        _lightPosition = new Vector3(_camera.Position.X, _camera.Position.Y, _camera.Position.Z);
      }
      if (input.IsKeyDown(Keys.Space))
      {
        _camera.Position += _camera.Up * _cameraSpeed * (float)e.Time; // Up
        _lightPosition = new Vector3(_camera.Position.X, _camera.Position.Y, _camera.Position.Z);
      }
      if (input.IsKeyDown(Keys.LeftShift))
      {
        _camera.Position -= _camera.Up * _cameraSpeed * (float)e.Time; // Down
        _lightPosition = new Vector3(_camera.Position.X, _camera.Position.Y, _camera.Position.Z);
      }
      if (input.IsKeyPressed(Keys.F12))
      {
        if (WindowState == WindowState.Fullscreen)
          WindowState = WindowState.Normal;
        else
          WindowState = WindowState.Fullscreen;
      }


      if (_firstMove)
      {
        _lastPos = new Vector2(mouse.X, mouse.Y);
        _firstMove = false;
      }
      else
      {
        var deltaX = mouse.X - _lastPos.X;
        var deltaY = mouse.Y - _lastPos.Y;
        _lastPos = new Vector2(mouse.X, mouse.Y);

        _camera.Yaw += deltaX * sensitivity;
        _camera.Pitch -= deltaY * sensitivity;
      }
    }

    protected override void OnMouseWheel(MouseWheelEventArgs e)
    {
      base.OnMouseWheel(e);

      _camera.Fov -= e.OffsetY;
    }

    private void SetSettings()
    {
      if (_settings == null) return;
      _viewingDistance = _settings.ViewingDistance ?? _viewingDistance;
      _polygonMode = _settings.PolygonMode ?? _polygonMode;
      _bgColor = _settings.BackgroundColor != null ? Helper.HexToColor4(_settings.BackgroundColor) : _bgColor;
      _linesScale = _settings.LinesScale ?? _linesScale;
      _cameraPosition = _settings.CameraPosition != null ? Helper.StringToVector3(_settings.CameraPosition) : _cameraPosition;
      _lightPosition = _cameraPosition;
      _modelColor = _settings.ModelColor != null ? Helper.HexToVector3(_settings.ModelColor) : _modelColor;
      _modelPosition = _settings.ModelPosition != null ? Helper.StringToVector3(_settings.ModelPosition) : _modelPosition;
      _modelRotation = _settings.ModelRotation ?? _modelRotation;
      _modelScale = _settings.ModelScale ?? _modelScale;
      _pathModel = _settings.ModelPath != null ? Helper.StringToPathModel(_settings.ModelPath) : _pathModel;
    }
    
    private float[] ParseFileLines()
    {
      parserLines.LoadObj(_pathLines);
      var vertices = new List<float>();
      foreach (var face in parserLines.VertexList)
      {
        vertices.Add(face.X);
        vertices.Add(face.Y);
        vertices.Add(face.Z);
      }
      return [.. vertices];
    }

    private ParseData ParseFileLamp()
    {
      parserLamp.LoadObj(_pathLamp);
      var vertices = new List<float>();
      var indices = new List<uint>();
      foreach (var face in parserLamp.FaceList)
      {
        for (int i = 0; i < 3; i++)
        {
          int vertexIndex = face.VertexIndexList[i] - 1;
          vertices.Add(parserLamp.VertexList[vertexIndex].X);
          vertices.Add(parserLamp.VertexList[vertexIndex].Y);
          vertices.Add(parserLamp.VertexList[vertexIndex].Z);

          indices.Add((uint)vertexIndex);
        }
      }
      return new ParseData { vertices = [.. vertices], indices = [.. indices] };
    }

    private ParseData ParseFileModel()
    {
      parserModel.LoadObj(_pathModel);
      var vertices = new List<float>();
      var indices = new List<uint>();
      foreach (var face in parserModel.FaceList)
      {
        for (int i = 0; i < 3; i++)
        {
          int vertexIndex = face.VertexIndexList[i] - 1;
          int normalsIndex = face.NormalsVertexIndexList[i] - 1;
          vertices.Add(parserModel.VertexList[vertexIndex].X);
          vertices.Add(parserModel.VertexList[vertexIndex].Y);
          vertices.Add(parserModel.VertexList[vertexIndex].Z);

          vertices.Add(parserModel.NormalsList[normalsIndex].X);
          vertices.Add(parserModel.NormalsList[normalsIndex].Y);
          vertices.Add(parserModel.NormalsList[normalsIndex].Z);

          indices.Add((uint)vertexIndex);
        }
      }
      return new ParseData { vertices = [.. vertices], indices = [.. indices] };
    }

    protected override void OnUnload()
    {
      GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
      GL.BindVertexArray(0);
      GL.UseProgram(0);
      base.OnUnload();
    }

    protected override void OnResize(ResizeEventArgs e)
    {
      base.OnResize(e);
      GL.Viewport(0, 0, Size.X, Size.Y);
      _camera.AspectRatio = Size.X / (float)Size.Y;
    }
  }
}
