#define CG_OpenGL
#define CG_Debug
// #define CG_DirectX

using CG_Biblioteca;
using OpenTK.Graphics.OpenGL4;
using System;
using System.Collections.Generic;
using OpenTK.Mathematics;

namespace gcgcg
{
  internal class Objeto
  {
    // Objeto
    private readonly char rotulo;
    protected Objeto paiRef;
    private List<Objeto> objetosLista = new List<Objeto>();
    private PrimitiveType primitivaTipo = PrimitiveType.LineLoop;
    public PrimitiveType PrimitivaTipo { get => primitivaTipo; set => primitivaTipo = value; }
    private float primitivaTamanho = 1;
    public float PrimitivaTamanho { get => primitivaTamanho; set => primitivaTamanho = value; }
    private Shader _shaderObjeto = new Shader("Shaders/shader.vert", "Shaders/shaderBranca.frag");
    public Shader shaderCor { set => _shaderObjeto = value; }

    // Vértices do objeto TODO: o objeto mundo deveria ter estes atributos abaixo?
    protected List<Ponto4D> pontosLista = new List<Ponto4D>();
    private int _vertexBufferObject;
    private int _vertexArrayObject;

    // BBox do objeto
    private BBox bBox = new BBox();
    public BBox Bbox()  // FIXME: readonly
    {
      return bBox;
    }

    // Transformações do objeto
    private Transformacao4D matriz = new Transformacao4D();

    /// Matrizes temporarias que sempre sao inicializadas com matriz Identidade entao podem ser "static".
    private static Transformacao4D matrizTmpTranslacao = new Transformacao4D();
    private static Transformacao4D matrizTmpTranslacaoInversa = new Transformacao4D();
    private static Transformacao4D matrizTmpEscala = new Transformacao4D();
    private static Transformacao4D matrizTmpRotacao = new Transformacao4D();
    private static Transformacao4D matrizGlobal = new Transformacao4D();
    private char eixoRotacao = 'z';
    public void TrocaEixoRotacao(char eixo) => eixoRotacao = eixo;

    public int index1 = 0, index2 = 1;


    public Objeto(Objeto paiRef, ref char _rotulo, Objeto objetoFilho = null)
    {
      this.paiRef = paiRef;
      rotulo = _rotulo = Utilitario.CharProximo(_rotulo);
      if (paiRef != null)
      {
        ObjetoNovo(objetoFilho);
      }
    }

    private void ObjetoNovo(Objeto objetoFilho)
    {
      if (objetoFilho == null)
      {
        paiRef.objetosLista.Add(this);
      }
      else
      {
        paiRef.FilhoAdicionar(objetoFilho);
      }
    }

    public bool VerificarInterseccao(Ponto4D pto)
    {
      return CalcularInterseccao(pto) % 2 != 0;
    }

    private int CalcularInterseccao(Ponto4D pto)
    {
      int count = 0;
      for (int i = 0; i < pontosLista.Count - 1; i++)
      {
        double numerador = pto.Y - pontosLista[i].Y;
        double denominador = pontosLista[i + 1].Y - pontosLista[i].Y;
        if (denominador != 0)
        {
          double t = numerador / denominador;

          if (t >= 0 && t <= 1)
          {
            double x = pontosLista[i].X + (pontosLista[i + 1].X - pontosLista[i].X) * t;
            if (x > pto.X)
            {
              count++;
            }
          }
        }
      }
      return count;
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
      bBox.Atualizar(pontosLista);

      GL.PointSize(primitivaTamanho);

      _vertexBufferObject = GL.GenBuffer();
      GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferObject);
      GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);
      _vertexArrayObject = GL.GenVertexArray();
      GL.BindVertexArray(_vertexArrayObject);
      GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
      GL.EnableVertexAttribArray(0);
    }

    // FIXME: falta para Transformações Geométricas PushMatrix e PopMatrix - Grafo de Cena
    public void Desenhar()
    {
#if CG_OpenGL && !CG_DirectX
      GL.PointSize(primitivaTamanho);

      GL.BindVertexArray(_vertexArrayObject);

      _shaderObjeto.SetMatrix4("transform", matriz.ObterDadosOpenTK());

      _shaderObjeto.Use();
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

    #region Objeto: CRUD

    public void FilhoAdicionar(Objeto filho)
    {
      this.objetosLista.Add(filho);
    }

     public void FilhoRemover(Objeto filho)
    {
      if (filho.rotulo != '@') this.objetosLista.Remove(filho);
    }

    public bool EhVazio()
    {
      return this.objetosLista.Count == 0;
    }

    public Ponto4D PontosId(int id)
    {
      return pontosLista[id];
    }

    public void PontosAdicionar(Ponto4D pto)
    {
      pontosLista.Add(pto);
    }

    public void PontosAlterar(Ponto4D pto, int posicao)
    {
      pontosLista[posicao] = pto;
    }

    #endregion

    #region Objeto: Grafo de Cena

    public Objeto GrafocenaBusca(char _rotulo)
    {
      if (rotulo == _rotulo)
      {
        return this;
      }
      foreach (var objeto in objetosLista)
      {
        var obj = objeto.GrafocenaBusca(_rotulo);
        if (obj != null)
        {
          return obj;
        }
      }
      return null;
    }

    public Objeto GrafocenaBuscaProximo(Objeto objetoAtual)
    {
      objetoAtual = GrafocenaBusca(Utilitario.CharProximo(objetoAtual.rotulo));
      if (objetoAtual != null)
      {
        return objetoAtual;
      }
      else
      {
        return GrafocenaBusca(Utilitario.CharProximo('@'));
      }
    }
    public void GrafocenaImprimir(String idt)
    {
      System.Console.WriteLine(idt + rotulo);
      foreach (var objeto in objetosLista)
      {
        objeto.GrafocenaImprimir(idt + "  ");
      }
    }

    #endregion

    #region Objeto: Transformações Geométricas

    public void MatrizImprimir()
    {
      System.Console.WriteLine(matriz);
    }
    public void MatrizAtribuirIdentidade()
    {
      matriz.AtribuirIdentidade();
      ObjetoAtualizar();
    }
    public void MatrizTranslacaoXYZ(double tx, double ty, double tz)
    {
      Transformacao4D matrizTranslate = new Transformacao4D();
      matrizTranslate.AtribuirTranslacao(tx, ty, tz);
      matriz = matrizTranslate.MultiplicarMatriz(matriz);
      ObjetoAtualizar();
    }
    public void MatrizEscalaXYZ(double Sx, double Sy, double Sz)
    {
      Transformacao4D matrizScale = new Transformacao4D();
      matrizScale.AtribuirEscala(Sx, Sy, Sz);
      matriz = matrizScale.MultiplicarMatriz(matriz);
      ObjetoAtualizar();
    }

    public void MatrizEscalaXYZBBox(double Sx, double Sy, double Sz)
    {
      matrizGlobal.AtribuirIdentidade();
      Ponto4D pontoPivo = bBox.obterCentro;

      matrizTmpTranslacao.AtribuirTranslacao(-pontoPivo.X, -pontoPivo.Y, -pontoPivo.Z); // Inverter sinal
      matrizGlobal = matrizTmpTranslacao.MultiplicarMatriz(matrizGlobal);

      matrizTmpEscala.AtribuirEscala(Sx, Sy, Sz);
      matrizGlobal = matrizTmpEscala.MultiplicarMatriz(matrizGlobal);

      matrizTmpTranslacaoInversa.AtribuirTranslacao(pontoPivo.X, pontoPivo.Y, pontoPivo.Z);
      matrizGlobal = matrizTmpTranslacaoInversa.MultiplicarMatriz(matrizGlobal);

      matriz = matriz.MultiplicarMatriz(matrizGlobal);

      ObjetoAtualizar();
    }
    public void MatrizRotacaoEixo(double angulo)
    {
      switch (eixoRotacao)  // FIXME: ainda não uso no exemplo
      {
        case 'x':
          matrizTmpRotacao.AtribuirRotacaoX(Transformacao4D.DEG_TO_RAD * angulo);
          break;
        case 'y':
          matrizTmpRotacao.AtribuirRotacaoY(Transformacao4D.DEG_TO_RAD * angulo);
          break;
        case 'z':
          matrizTmpRotacao.AtribuirRotacaoZ(Transformacao4D.DEG_TO_RAD * angulo);
          break;
        default:
          Console.WriteLine("opção de eixoRotacao: ERRADA!");
          break;
      }
      ObjetoAtualizar();
    }
    public void MatrizRotacao(double angulo)
    {
      MatrizRotacaoEixo(angulo);
      matriz = matrizTmpRotacao.MultiplicarMatriz(matriz);
      ObjetoAtualizar();
    }
    public void MatrizRotacaoZBBox(double angulo)
    {
      matrizGlobal.AtribuirIdentidade();
      Ponto4D pontoPivo = bBox.obterCentro;

      matrizTmpTranslacao.AtribuirTranslacao(-pontoPivo.X, -pontoPivo.Y, -pontoPivo.Z); // Inverter sinal
      matrizGlobal = matrizTmpTranslacao.MultiplicarMatriz(matrizGlobal);

      MatrizRotacaoEixo(angulo);
      matrizGlobal = matrizTmpRotacao.MultiplicarMatriz(matrizGlobal);

      matrizTmpTranslacaoInversa.AtribuirTranslacao(pontoPivo.X, pontoPivo.Y, pontoPivo.Z);
      matrizGlobal = matrizTmpTranslacaoInversa.MultiplicarMatriz(matrizGlobal);

      matriz = matriz.MultiplicarMatriz(matrizGlobal);

      ObjetoAtualizar();
    }

    #endregion

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

      GL.DeleteProgram(_shaderObjeto.Handle);
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
      retorno += bBox.ToString();
      return (retorno);
    }
#endif

  }
}