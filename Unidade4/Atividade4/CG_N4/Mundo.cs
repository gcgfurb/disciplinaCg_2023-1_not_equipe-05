﻿#define CG_Gizmo  // debugar gráfico.
#define CG_OpenGL // render OpenGL.
// #define CG_DirectX // render DirectX.
// #define CG_Privado // código do professor.

using CG_Biblioteca;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Windowing.Desktop;
using System;
using OpenTK.Mathematics;
using System.Collections.Generic;


//FIXME: padrão Singleton

namespace gcgcg
{
  public class Mundo : GameWindow
  {
    Objeto mundo;
    private char rotuloNovo = '?';
    private Objeto objetoSelecionado = null;

    private readonly float[] _sruEixos =
    {
      -0.5f,  0.0f,  0.0f, /* X- */      0.5f,  0.0f,  0.0f, /* X+ */
       0.0f, -0.5f,  0.0f, /* Y- */      0.0f,  0.5f,  0.0f, /* Y+ */
       0.0f,  0.0f, -0.5f, /* Z- */      0.0f,  0.0f,  0.5f  /* Z+ */
    };

    // private readonly float[] _vertices = {
    //     // Position         Texture coordinates
    //     -0.3005f, -0.3005f,  0.3005f, 0.0f, 0.0f,
    //      0.3005f, -0.3005f,  0.3005f, 1.0f, 0.0f,
    //      0.3005f,  0.3005f,  0.3005f, 1.0f, 1.0f,
    //     -0.3005f,  0.3005f,  0.3005f, 0.0f, 1.0f
    //   };

      private readonly float[] _vertices = {
        // Position         Texture coordinates
        -0.3005f, -0.3005f,  0.3005f, 0.0f, 0.0f, // front
          0.3005f, -0.3005f,  0.3005f, 1.0f, 0.0f,
          0.3005f,  0.3005f,  0.3005f, 1.0f, 1.0f,
        -0.3005f,  0.3005f,  0.3005f, 0.0f, 1.0f,
        -0.3005f, -0.3005f, -0.3005f, 0.0f, 0.0f, // back
        0.3005f, -0.3005f, -0.3005f, 1.0f, 0.0f,
        0.3005f,  0.3005f, -0.3005f, 1.0f, 1.0f,
        -0.3005f,  0.3005f, -0.3005f, 0.0f, 1.0f,
        -0.3005f, 0.3005f,  0.3005f, 0.0f, 0.0f, // top
        0.3005f, 0.3005f,  -0.3005f, 1.0f, 1.0f,
        0.3005f, 0.3005f,  0.3005f, 1.0f, 0.0f,
        -0.3005f, 0.3005f, -0.3005f, 0.0f, 1.0f,
        0.3005f, -0.3005f,  -0.3005f, 0.0f, 0.0f, // bottom
        -0.3005f, -0.3005f,  0.3005f, 1.0f, 1.0f,
        -0.3005f, -0.3005f,  -0.3005f, 1.0f, 0.0f,
        0.3005f, -0.3005f, 0.3005f, 0.0f, 1.0f,
        0.3005f, -0.3005f, -0.3005f, 0.0f, 0.0f, //right
        0.3005f, 0.3005f, 0.3005f, 1.0f, 1.0f,
        0.3005f, -0.3005f, 0.3005f, 1.0f, 0.0f,
        0.3005f, 0.3005f, -0.3005f, 0.0f, 1.0f,
       -0.3005f, -0.3005f, 0.3005f, 0.0f, 0.0f, //left
       -0.3005f, 0.3005f, -0.3005f, 1.0f, 1.0f,
       -0.3005f, -0.3005f, -0.3005f, 1.0f, 0.0f,
       -0.3005f, 0.3005f, 0.3005f, 0.0f, 1.0f,
    };
    private readonly uint[] _indices =
    {
        1, 2, 3,
        0, 1, 3,
        5, 6, 7,
        4, 5, 7,
        11, 8, 9,
        8, 10, 9,
        15, 12, 13,
        12, 14, 13,
        17, 16, 18,
        19, 16, 17,
        21, 20, 22,
        23, 20, 21
    };

    private int _vertexBufferObject_sruEixos;
    private int _vertexArrayObject_sruEixos;

    private float xAtual;
    private float xAnterior;
    double anguloX;
    private float yAtual;
    private float yAnterior;
    double anguloY;

    private Shader _shader;
    private Shader _shaderBranca;
    private Shader _shaderVermelha;
    private Shader _shaderVerde;
    private Shader _shaderAzul;
    private Shader _shaderCiano;
    private Shader _shaderMagenta;
    private Shader _shaderAmarela;
    private Texture _texture;
    private Texture _texture2;
    private Texture _texture3;
    private Texture _texture4;
    private Texture _texture5;
    private Texture _texture6;
    private int _vertexBufferObject_texture;
    private int _vertexArrayObject_texture;
    private int _elementBufferObject_texture;

    private Camera _camera;

    public Mundo(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings)
           : base(gameWindowSettings, nativeWindowSettings)
    {
      mundo = new Objeto(null, ref rotuloNovo);
    }

    private void Diretivas()
    {
#if DEBUG
      Console.WriteLine("Debug version");
#endif      
#if RELEASE
    Console.WriteLine("Release version");
#endif      
#if CG_Gizmo      
      Console.WriteLine("#define CG_Gizmo  // debugar gráfico.");
#endif
#if CG_OpenGL      
      Console.WriteLine("#define CG_OpenGL // render OpenGL.");
#endif
#if CG_DirectX      
      Console.WriteLine("#define CG_DirectX // render DirectX.");
#endif
#if CG_Privado      
      Console.WriteLine("#define CG_Privado // código do professor.");
#endif
      Console.WriteLine("__________________________________ \n");
    }

    protected override void OnLoad()
    {
      base.OnLoad();

      Diretivas();

      GL.ClearColor(0.0f, 0.0f, 0.0f, 1.0f);

      GL.Enable(EnableCap.DepthTest);       // Ativar teste de profundidade
      // GL.Enable(EnableCap.CullFace);     // Desenha os dois lados da face
      // GL.FrontFace(FrontFaceDirection.Cw);
      // GL.CullFace(CullFaceMode.FrontAndBack);

      #region Cores
      _shaderBranca = new Shader("Shaders/shader.vert", "Shaders/shaderBranca.frag");
      _shaderVermelha = new Shader("Shaders/shader.vert", "Shaders/shaderVermelha.frag");
      _shaderVerde = new Shader("Shaders/shader.vert", "Shaders/shaderVerde.frag");
      _shaderAzul = new Shader("Shaders/shader.vert", "Shaders/shaderAzul.frag");
      _shaderCiano = new Shader("Shaders/shader.vert", "Shaders/shaderCiano.frag");
      _shaderMagenta = new Shader("Shaders/shader.vert", "Shaders/shaderMagenta.frag");
      _shaderAmarela = new Shader("Shaders/shader.vert", "Shaders/shaderAmarela.frag");
      #endregion

      #region Texturas

      GL.Enable(EnableCap.Texture2D);
      _vertexArrayObject_texture = GL.GenVertexArray();
      GL.BindVertexArray(_vertexArrayObject_texture);

      _vertexBufferObject_texture = GL.GenBuffer();
      GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferObject_texture);
      GL.BufferData(BufferTarget.ArrayBuffer, _vertices.Length * sizeof(float), _vertices, BufferUsageHint.StaticDraw);

      _elementBufferObject_texture = GL.GenBuffer();
      GL.BindBuffer(BufferTarget.ElementArrayBuffer, _elementBufferObject_texture);
      GL.BufferData(BufferTarget.ElementArrayBuffer, _indices.Length * sizeof(uint), _indices, BufferUsageHint.StaticDraw);

      _shader = new Shader("Shaders/shader_texture.vert", "Shaders/shader_texture.frag");
      _shader.Use();

      var vertexLocation = _shader.GetAttribLocation("aPosition");
      GL.EnableVertexAttribArray(vertexLocation);
      GL.VertexAttribPointer(vertexLocation, 3, VertexAttribPointerType.Float, false, 5 * sizeof(float), 0);

      var texCoordLocation = _shader.GetAttribLocation("aTexCoord");
      GL.EnableVertexAttribArray(texCoordLocation);
      GL.VertexAttribPointer(texCoordLocation, 2, VertexAttribPointerType.Float, false, 5 * sizeof(float), 3 * sizeof(float));

      _texture = Texture.LoadFromFile("Resources/all.jpg");
      _texture.Use(TextureUnit.Texture0);


      #endregion

      #region Eixos: SRU  
      _vertexBufferObject_sruEixos = GL.GenBuffer();
      GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferObject_sruEixos);
      GL.BufferData(BufferTarget.ArrayBuffer, _sruEixos.Length * sizeof(float), _sruEixos, BufferUsageHint.StaticDraw);
      _vertexArrayObject_sruEixos = GL.GenVertexArray();
      GL.BindVertexArray(_vertexArrayObject_sruEixos);
      GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
      GL.EnableVertexAttribArray(0);
      #endregion

      #region Objeto: polígono qualquer  
      List<Ponto4D> pontosPoligonoBandeira = new List<Ponto4D>();
      pontosPoligonoBandeira.Add(new Ponto4D(0.25, 0.25));
      pontosPoligonoBandeira.Add(new Ponto4D(0.75, 0.25));
      pontosPoligonoBandeira.Add(new Ponto4D(0.75, 0.75));
      pontosPoligonoBandeira.Add(new Ponto4D(0.50, 0.50));
      pontosPoligonoBandeira.Add(new Ponto4D(0.25, 0.75));
      objetoSelecionado = new Poligono(mundo, ref rotuloNovo, pontosPoligonoBandeira);
      #endregion
      #region declara um objeto filho ao polígono qualquer
      List<Ponto4D> pontosPoligonoTriangulo = new List<Ponto4D>();
      pontosPoligonoTriangulo.Add(new Ponto4D(0.50, 0.50));
      pontosPoligonoTriangulo.Add(new Ponto4D(0.75, 0.75));
      pontosPoligonoTriangulo.Add(new Ponto4D(0.25, 0.75));
      objetoSelecionado = new Poligono(objetoSelecionado, ref rotuloNovo, pontosPoligonoTriangulo);
      objetoSelecionado.PrimitivaTipo = PrimitiveType.Triangles;
      #endregion

      #region Objeto: polígono quadrado
      List<Ponto4D> pontosPoligonoQuadrado = new List<Ponto4D>();
      pontosPoligonoQuadrado.Add(new Ponto4D(-0.25, 0.25, 0.1));
      pontosPoligonoQuadrado.Add(new Ponto4D(-0.75, 0.25, 0.1));
      pontosPoligonoQuadrado.Add(new Ponto4D(-0.75, 0.75, 0.1));
      pontosPoligonoQuadrado.Add(new Ponto4D(-0.25, 0.75, 0.1));
      objetoSelecionado = new Poligono(mundo, ref rotuloNovo, pontosPoligonoQuadrado);
      objetoSelecionado.PrimitivaTipo = PrimitiveType.TriangleFan;
      #endregion

      #region Objeto: Cubo
      objetoSelecionado = new Cubo(mundo, ref rotuloNovo);
      objetoSelecionado.shaderCor = _shader;
      objetoSelecionado.PrimitivaTipo = PrimitiveType.TriangleFan;

      #endregion

      _camera = new Camera(Vector3.UnitZ, Size.X / (float)Size.Y);

      anguloX = -90;
      anguloY = 0;

    }

    protected override void OnRenderFrame(FrameEventArgs e)
    {
      base.OnRenderFrame(e);

      GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

      mundo.Desenhar(new Transformacao4D(), _camera);
      #region drawImageFront
      
      GL.BindVertexArray(_vertexArrayObject_texture);

      _texture.Use(TextureUnit.Texture0);
      _shader.Use();
      GL.DrawElements(PrimitiveType.Triangles, _indices.Length, DrawElementsType.UnsignedInt, 0);
      GL.DrawElements(PrimitiveType.Points, _indices.Length, DrawElementsType.UnsignedInt, 0);

      #endregion

      

#if CG_Gizmo      
      Gizmo_Sru3D();
#endif
      SwapBuffers();
    }

      public double converteValorPonto(float coordenada, bool eixo){
      // coordenada vem um valor entre 0 e 800, tem que converter pra -1 até 1
      // eixo true = x, eixo false = y
      float convertido;
      float inc = 0.0025f;
      if (eixo){
        convertido = -1+(inc*coordenada);
      } else {
        convertido = +1-(inc*coordenada);
      }
      return convertido;
    }

    protected override void OnUpdateFrame(FrameEventArgs e)
    {
      base.OnUpdateFrame(e);

      // ☞ 396c2670-8ce0-4aff-86da-0f58cd8dcfdc   TODO: forma otimizada para teclado.
      #region Teclado
      var input = KeyboardState;
      if (input.IsKeyDown(Keys.Escape))
        Close();
      if (input.IsKeyPressed(Keys.Space))
      {
        if (objetoSelecionado == null)
          objetoSelecionado = mundo;
        if (objetoSelecionado.shaderCor != _shader)
          objetoSelecionado.shaderCor = _shaderBranca;
        objetoSelecionado = mundo.GrafocenaBuscaProximo(objetoSelecionado);
        if (objetoSelecionado.shaderCor != _shader)
          objetoSelecionado.shaderCor = _shaderAmarela;
      }
      if (input.IsKeyPressed(Keys.G))
        mundo.GrafocenaImprimir("");
      if (input.IsKeyPressed(Keys.P) && objetoSelecionado != null)
        System.Console.WriteLine(objetoSelecionado.ToString());
      if (input.IsKeyPressed(Keys.M) && objetoSelecionado != null)
        objetoSelecionado.MatrizImprimir();
      if (input.IsKeyPressed(Keys.I) && objetoSelecionado != null)
        objetoSelecionado.MatrizAtribuirIdentidade();
      if (input.IsKeyPressed(Keys.Left) && objetoSelecionado != null)
        objetoSelecionado.MatrizTranslacaoXYZ(-0.05, 0, 0);
      if (input.IsKeyPressed(Keys.Right) && objetoSelecionado != null)
        objetoSelecionado.MatrizTranslacaoXYZ(0.05, 0, 0);
      if (input.IsKeyPressed(Keys.Up) && objetoSelecionado != null)
        objetoSelecionado.MatrizTranslacaoXYZ(0, 0.05, 0);
      if (input.IsKeyPressed(Keys.Down) && objetoSelecionado != null)
        objetoSelecionado.MatrizTranslacaoXYZ(0, -0.05, 0);
      if (input.IsKeyPressed(Keys.O) && objetoSelecionado != null)
        objetoSelecionado.MatrizTranslacaoXYZ(0, 0, 0.05);
      if (input.IsKeyPressed(Keys.L) && objetoSelecionado != null)
        objetoSelecionado.MatrizTranslacaoXYZ(0, 0, -0.05);
      if (input.IsKeyPressed(Keys.PageUp) && objetoSelecionado != null)
        objetoSelecionado.MatrizEscalaXYZ(2, 2, 2);
      if (input.IsKeyPressed(Keys.PageDown) && objetoSelecionado != null)
        objetoSelecionado.MatrizEscalaXYZ(0.5, 0.5, 0.5);
      if (input.IsKeyPressed(Keys.Home) && objetoSelecionado != null)
        objetoSelecionado.MatrizEscalaXYZBBox(0.5, 0.5, 0.5);
      if (input.IsKeyPressed(Keys.End) && objetoSelecionado != null)
        objetoSelecionado.MatrizEscalaXYZBBox(2, 2, 2);
      if (input.IsKeyPressed(Keys.D1) && objetoSelecionado != null)
        objetoSelecionado.MatrizRotacao(10);
      if (input.IsKeyPressed(Keys.D2) && objetoSelecionado != null)
        objetoSelecionado.MatrizRotacao(-10);
      if (input.IsKeyPressed(Keys.D3) && objetoSelecionado != null)
        objetoSelecionado.MatrizRotacaoZBBox(10);
      if (input.IsKeyPressed(Keys.D4) && objetoSelecionado != null)
        objetoSelecionado.MatrizRotacaoZBBox(-10);

      const float cameraSpeed = 1.5f;
      if (input.IsKeyDown(Keys.Z))
        _camera.Position = Vector3.UnitZ;
      if (input.IsKeyDown(Keys.W))
        _camera.Position += _camera.Front * cameraSpeed * (float)e.Time; // Forward
      if (input.IsKeyDown(Keys.S))
        _camera.Position -= _camera.Front * cameraSpeed * (float)e.Time; // Backwards
      if (input.IsKeyDown(Keys.A))
        _camera.Position -= _camera.Right * cameraSpeed * (float)e.Time; // Left
      if (input.IsKeyDown(Keys.D))
        _camera.Position += _camera.Right * cameraSpeed * (float)e.Time; // Right
      if (input.IsKeyDown(Keys.RightShift))
        _camera.Position += _camera.Up * cameraSpeed * (float)e.Time; // Up
      if (input.IsKeyDown(Keys.LeftShift))
        _camera.Position -= _camera.Up * cameraSpeed * (float)e.Time; // Down
      if (input.IsKeyDown(Keys.H))
        _camera.Pitch += -0.5f;
      if (input.IsKeyDown(Keys.Y))
        _camera.Pitch += 0.5f;
      if (input.IsKeyDown(Keys.G))
        _camera.Yaw += -0.5f;
      if (input.IsKeyDown(Keys.J))
        _camera.Yaw += 0.5f;
      #endregion

      #region  Mouse

      if (MouseState.IsButtonPressed(MouseButton.Left))
      {
        System.Console.WriteLine("MouseState.IsButtonPressed(MouseButton.Left)");
        System.Console.WriteLine("__ Valores do Espaço de Tela");
        System.Console.WriteLine("Vector2 mousePosition: " + MousePosition);
        // System.Console.WriteLine("X: " + MouseState.X +" | PreviousX: "+ MouseState.PreviousX);
        System.Console.WriteLine("Vector2i windowSize: " + Size);

        xAnterior = MouseState.X;
        yAnterior = MouseState.Y;
      }
      if (MouseState.IsButtonDown(MouseButton.Left))
      {
        xAtual = MouseState.X;
        yAtual = MouseState.Y;
        // Anterior < Atual = gira Anti-horário ("pra direita/pra cima")
        // Anterior > Atual = gira horário ("pra esquerda/pra baixo")

        // vai pegar um angulo entre 0 e 360°
        // multiplicando por 0.45f = 800/360 (regra de 3)
        // dividindo isso por 100 pq se n girava o cubo muito rapido
        float incrementoX = (xAnterior - xAtual)*0.0045f;
        float incrementoY = (yAnterior - yAtual)*0.0045f;
        anguloX += incrementoX;
        anguloY += incrementoY;
        _camera.AtualizarCamera(anguloX, incrementoX, anguloY, incrementoY);
      }
      if (MouseState.IsButtonDown(MouseButton.Right) && objetoSelecionado != null)
      {
        System.Console.WriteLine("MouseState.IsButtonDown(MouseButton.Right)");

        int janelaLargura = Size.X;
        int janelaAltura = Size.Y;
        Ponto4D mousePonto = new Ponto4D(MousePosition.X, MousePosition.Y);
        Ponto4D sruPonto = Utilitario.NDC_TelaSRU(janelaLargura, janelaAltura, mousePonto);

        objetoSelecionado.PontosAlterar(sruPonto, 0);
      }
      if (MouseState.IsButtonReleased(MouseButton.Right))
      {
        System.Console.WriteLine("MouseState.IsButtonReleased(MouseButton.Right)");
      }

      #endregion

    }

    protected override void OnResize(ResizeEventArgs e)
    {
      base.OnResize(e);

      GL.Viewport(0, 0, Size.X, Size.Y);
    }

    protected override void OnUnload()
    {
      mundo.OnUnload();

      GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
      GL.BindVertexArray(0);
      GL.UseProgram(0);

      GL.DeleteBuffer(_vertexBufferObject_sruEixos);
      GL.DeleteVertexArray(_vertexArrayObject_sruEixos);

      GL.DeleteProgram(_shaderBranca.Handle);
      GL.DeleteProgram(_shaderVermelha.Handle);
      GL.DeleteProgram(_shaderVerde.Handle);
      GL.DeleteProgram(_shaderAzul.Handle);
      GL.DeleteProgram(_shaderCiano.Handle);
      GL.DeleteProgram(_shaderMagenta.Handle);
      GL.DeleteProgram(_shaderAmarela.Handle);

      base.OnUnload();
    }

#if CG_Gizmo
    private void Gizmo_Sru3D()
    {
#if CG_OpenGL && !CG_DirectX
      var model = Matrix4.Identity;
      GL.BindVertexArray(_vertexArrayObject_sruEixos);
      // EixoX
      _shaderVermelha.SetMatrix4("model", model);
      _shaderVermelha.SetMatrix4("view", _camera.GetViewMatrix());
      _shaderVermelha.SetMatrix4("projection", _camera.GetProjectionMatrix());
      _shaderVermelha.Use();
      GL.DrawArrays(PrimitiveType.Lines, 0, 2);
      // EixoY
      _shaderVerde.SetMatrix4("model", model);
      _shaderVerde.SetMatrix4("view", _camera.GetViewMatrix());
      _shaderVerde.SetMatrix4("projection", _camera.GetProjectionMatrix());
      _shaderVerde.Use();
      GL.DrawArrays(PrimitiveType.Lines, 2, 2);
      // EixoZ
      _shaderAzul.SetMatrix4("model", model);
      _shaderAzul.SetMatrix4("view", _camera.GetViewMatrix());
      _shaderAzul.SetMatrix4("projection", _camera.GetProjectionMatrix());
      _shaderAzul.Use();
      GL.DrawArrays(PrimitiveType.Lines, 4, 2);

#elif CG_DirectX && !CG_OpenGL
      Console.WriteLine(" .. Coloque aqui o seu código em DirectX");
#elif (CG_DirectX && CG_OpenGL) || (!CG_DirectX && !CG_OpenGL)
      Console.WriteLine(" .. ERRO de Render - escolha OpenGL ou DirectX !!");
#endif
    }
#endif    

  }
}
