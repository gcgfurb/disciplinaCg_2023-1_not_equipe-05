#define CG_OpenGL
#define CG_Debug
// #define CG_DirectX

using CG_Biblioteca;
using OpenTK.Graphics.OpenGL4;
using System.Collections.Generic;
using System;

namespace gcgcg
{
  internal class Objeto
  {
    // Objeto
    protected char rotulo;
    public char Rotulo { get => rotulo; set => rotulo = value; }
    protected Objeto paiRef;
    public Objeto PaiRef { get => paiRef; }
    private List<Objeto> objetosLista = new List<Objeto>();
    private PrimitiveType primitivaTipo = PrimitiveType.LineLoop;
    public PrimitiveType PrimitivaTipo { get => primitivaTipo; set => primitivaTipo = value; }
    private float primitivaTamanho = 1;
    public float PrimitivaTamanho { get => primitivaTamanho; set => primitivaTamanho = value; }
    private Shader _shaderCor = new Shader("Shaders/shader.vert", "Shaders/shaderBranca.frag");
    public Shader shaderCor { set => _shaderCor = value; }

    // Vértices do objeto
    protected List<Ponto4D> pontosLista = new List<Ponto4D>();
    private int _vertexBufferObject;
    private int _vertexArrayObject;

    public Objeto(Objeto paiRef)
    {
      this.rotulo = '@';
      this.paiRef = paiRef;
    }

    public List<Objeto> GetObjetosLista(){
      return objetosLista;
    }

    public List<Objeto> GetObjetosLista(Type t){
      List<Objeto> temp = new List<Objeto>();
      foreach (Objeto obj in GetObjetosLista()) {
        if (obj.GetType() == t) {
          temp.Add(obj);
        }
      }

      return temp;
    }
    public void ObjetoAtualizar()
    {
      float[] vertices = new float[pontosLista.Count * 3];
      int ptoLista = 0;
      for (int i = 0; i < vertices.Length; i += 3)
      {
        vertices[i] = (float)pontosLista[ptoLista].X;
        vertices[i + 1] = (float)pontosLista[ptoLista].Y;
        vertices[i + 2] = (float)pontosLista[ptoLista].Z;
        ptoLista++;
      }

      _vertexBufferObject = GL.GenBuffer();
      GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferObject);
      GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);
      _vertexArrayObject = GL.GenVertexArray();
      GL.BindVertexArray(_vertexArrayObject);
      GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
      GL.EnableVertexAttribArray(0);
    }

    public virtual void Desenhar()
    {
#if CG_OpenGL && !CG_DirectX
      GL.PointSize(primitivaTamanho);
      GL.BindVertexArray(_vertexArrayObject);
      _shaderCor.Use();
      GL.DrawArrays(primitivaTipo, 0, pontosLista.Count);
#elif CG_DirectX && !CG_OpenGL
      Console.WriteLine(" .. Coloque aqui o seu código em DirectX");
#elif (CG_DirectX && CG_OpenGL) || (!CG_DirectX && !CG_OpenGL)
      Console.WriteLine(" .. ERRO de Render - escolha OpenGL ou DirectX !!");
#endif
      for (var i = 0; i < objetosLista.Count; i++)
      {
        objetosLista[i].Desenhar();
      }
    }

    public void FilhoAdicionar(Objeto filho)
    {
      this.objetosLista.Add(filho);
    }

    public void FilhoRemover(Objeto filho)
    {
      this.objetosLista.Remove(filho);
    }


    public void PontosAdicionar(Ponto4D pto)
    {
      pontosLista.Add(pto);
    }

    public List<Ponto4D> GetPontos() {
      return pontosLista;
    }


    public Ponto4D PontosId(int id)
    {
      return pontosLista[id];
    }

    public void PontosLimpar(){
      pontosLista.Clear();
    }

    public void PontosRemover(int pos) {
      pontosLista.RemoveAt(pos);
    }

    public void PontosRemover(Ponto4D pto) {
      pontosLista.Remove(pto);
    }
    public void PontosAlterar(Ponto4D pto, int posicao)
    {
      pontosLista[posicao] = pto;
    }

    public void OnUnload()
    {
      foreach (var objeto in objetosLista)
      {
        objeto.OnUnload();
      }

      GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
      GL.BindVertexArray(0);
      GL.UseProgram(0);

      GL.DeleteBuffer(_vertexBufferObject);
      GL.DeleteVertexArray(_vertexArrayObject);

      GL.DeleteProgram(_shaderCor.Handle);
    }

#if CG_Debug
    protected string ImprimeToString()
    {
      string retorno;
      retorno = "__ Objeto: " + rotulo + "\n";
      for (var i = 0; i < pontosLista.Count; i++)
      {
        retorno += "P" + i + "[ " +
        string.Format("{0,10}", pontosLista[i].X) + " | " +
        string.Format("{0,10}", pontosLista[i].Y) + " | " +
        string.Format("{0,10}", pontosLista[i].Z) + " | " +
        string.Format("{0,10}", pontosLista[i].W) + " ]" + "\n";
      }
      return (retorno);
    }
#endif

  }
}