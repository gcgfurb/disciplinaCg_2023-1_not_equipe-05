#define CG_Gizmo  // debugar gráfico.
#define CG_OpenGL // render OpenGL.
//#define CG_DirectX // render DirectX.
// #define CG_Privado // código do professor.

using CG_Biblioteca;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Windowing.Desktop;
using System;
using System.Collections.Generic;
using OpenTK.Mathematics;

//FIXME: padrão Singleton

namespace gcgcg
{
    public class Mundo : GameWindow
    {
        private List<Objeto> objetosLista = new List<Objeto>();

        private Objeto objetoSelecionado = null;

        private List<Objeto> ptosControle = new List<Objeto>();
        private Objeto 
            p1,
            p2,
            p3,
            p4,
            s1,
            s2,
            s3;
        private Spline spline;
        private int contador,
            tamanhoLista = 0;
        private int qntPtos = 10;
        private char rotulo = '@';
        private SegReta ultimaReta;

        private readonly float[] _sruEixos =
        {
            0.0f,
            0.0f,
            0.0f, /* X- */
            0.5f,
            0.0f,
            0.0f, /* X+ */
            0.0f,
            0.0f,
            0.0f, /* Y- */
            0.0f,
            0.5f,
            0.0f, /* Y+ */
            0.0f,
            0.0f,
            0.0f, /* Z- */
            0.0f,
            0.0f,
            0.5f, /* Z+ */
        };

        private int _vertexBufferObject_sruEixos;
        private int _vertexArrayObject_sruEixos;

        private Shader _shaderVermelha;
        private Shader _shaderVerde;
        private Shader _shaderAzul;

        private bool _firstMove = true;
        private Vector2 _lastPos;

        public Mundo(
            GameWindowSettings gameWindowSettings,
            NativeWindowSettings nativeWindowSettings
        )
            : base(gameWindowSettings, nativeWindowSettings) { }

        
        private void RemoverPontoSpline() {
            List<Objeto> segRetas = spline.GetObjetosLista(typeof(SegReta));
            objetosLista.Remove(ultimaReta);
            spline.FilhoRemover(ultimaReta);
            segRetas.Remove(ultimaReta);

            ultimaReta = (SegReta) segRetas[segRetas.Count - 1];
            ultimaReta.PontosAlterar(ptosControle[3].PontosId(0), 1);

            ultimaReta.ObjetoAtualizar();

            qntPtos--;
            spline.SetQntPontos(qntPtos);
            spline.PontosRemover(qntPtos - 1);

            AtualizarSpline();
        }

        private void AdicionarPontoSpline() {
            List<Objeto> segRetas = spline.GetObjetosLista(typeof(SegReta));

            for (int i = 0; i < segRetas.Count-1; i++) {
                spline.FilhoRemover(segRetas[i]);
            }

            for (int i = 0; i < spline.GetPontos().Count - 1; i++)
            {
                ObjetoNovo(spline, new SegReta(spline, spline.PontosId(i), spline.PontosId(i + 1)));
                objetoSelecionado.shaderCor = new Shader(
                    "Shaders/shader.vert",
                    "Shaders/shaderAmarela.frag"
                );
            }

            //última reta
            ultimaReta = new SegReta(
                spline,
                spline.PontosId(spline.GetPontos().Count - 1),
                ptosControle[3].PontosId(0)
            );
            ultimaReta.shaderCor = new Shader("Shaders/shader.vert", "Shaders/shaderAmarela.frag");
            ObjetoNovo(spline, ultimaReta);

            qntPtos++;
            spline.SetQntPontos(qntPtos);
            spline.PontosAdicionar(ptosControle[3].PontosId(0));

            AtualizarSpline();
        }
        
        private void AtualizarSpline()
        {
            spline.PontosAlterar(p1.PontosId(0), 0);
            spline.PontosAlterar(p2.PontosId(0), 1);
            spline.PontosAlterar(p3.PontosId(0), 2);
            spline.PontosAlterar(p4.PontosId(0), 3);
            spline.Desenhar();

            List<Objeto> segRetas = spline.GetObjetosLista(typeof(SegReta));

            for (int i = 0; i < segRetas.Count - 1; i++)
            {
                segRetas[i].PontosAlterar(spline.PontosId(i), 0);
                segRetas[i].PontosAlterar(spline.PontosId(i + 1), 1);

                segRetas[i].ObjetoAtualizar();
            }

            ultimaReta.PontosAlterar(spline.PontosId(spline.GetPontos().Count - 1), 0);
            ultimaReta.PontosAlterar(ptosControle[3].PontosId(0), 1);
            ultimaReta.ObjetoAtualizar();
        }

        private void ObjetoNovo(Objeto objeto, Objeto objetoFilho = null)
        {
            if (objetoFilho == null)
            {
                objetosLista.Add(objeto);
                objeto.Rotulo = rotulo = Utilitario.charProximo(rotulo);
                objeto.ObjetoAtualizar();
                objetoSelecionado = objeto;
            }
            else
            {
                objeto.FilhoAdicionar(objetoFilho);
                objetoFilho.Rotulo = rotulo = Utilitario.charProximo(rotulo);
                objetoFilho.ObjetoAtualizar();
                objetoSelecionado = objetoFilho;
            }
        }

        private List<Objeto> GetObjectsByParent(Objeto paiRef, Type t)
        {
            List<Objeto> temp = new List<Objeto>();
            foreach (Objeto obj in objetosLista)
            {
                if (obj.PaiRef == paiRef && obj.GetType() == t)
                {
                    temp.Add(obj);
                }
            }

            return temp;
        }

        private void getSegReta(int contador, Ponto4D pto)
        {
            switch (contador)
            {
                case 0:
                    p1.PontosAlterar(pto, 0);
                    p1.ObjetoAtualizar();
                    s1.PontosAlterar(pto, 0);
                    s1.ObjetoAtualizar();
                    break;
                case 1:
                    s1.PontosAlterar(pto, 1);
                    s1.ObjetoAtualizar();
                    s2.PontosAlterar(pto, 0);
                    s2.ObjetoAtualizar();
                    break;
                case 2:
                    s2.PontosAlterar(pto, 1);
                    s2.ObjetoAtualizar();
                    s3.PontosAlterar(pto, 0);
                    s3.ObjetoAtualizar();
                    break;
                case 3:
                    s3.PontosAlterar(pto, 1);
                    s3.ObjetoAtualizar();
                    break;
                default:
                    break;
            }
        }

        protected override void OnLoad()
        {
            base.OnLoad();

            GL.ClearColor(0.5f, 0.5f, 0.5f, 1.0f);

            // Eixos
            _vertexBufferObject_sruEixos = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferObject_sruEixos);
            GL.BufferData(
                BufferTarget.ArrayBuffer,
                _sruEixos.Length * sizeof(float),
                _sruEixos,
                BufferUsageHint.StaticDraw
            );
            _vertexArrayObject_sruEixos = GL.GenVertexArray();
            GL.BindVertexArray(_vertexArrayObject_sruEixos);
            GL.VertexAttribPointer(
                0,
                3,
                VertexAttribPointerType.Float,
                false,
                3 * sizeof(float),
                0
            );
            GL.EnableVertexAttribArray(0);
            _shaderVermelha = new Shader("Shaders/shader.vert", "Shaders/shaderVermelha.frag");
            _shaderVerde = new Shader("Shaders/shader.vert", "Shaders/shaderVerde.frag");
            _shaderAzul = new Shader("Shaders/shader.vert", "Shaders/shaderAzul.frag");


            p1 = new Ponto(null, new Ponto4D(-0.5, -0.5));
            p2 = new Ponto(null, new Ponto4D(-0.5, 0.5));
            p3 = new Ponto(null, new Ponto4D(0.5, 0.5));
            p4 = new Ponto(null, new Ponto4D(0.5, -0.5));
            ObjetoNovo(p1);
            ObjetoNovo(p2);
            ObjetoNovo(p3);
            ObjetoNovo(p4);
            p1.shaderCor = _shaderVermelha;

            ptosControle.Add(p1);
            ptosControle.Add(p2);
            ptosControle.Add(p3);
            ptosControle.Add(p4);

            spline = new Spline(
                null,
                p1.PontosId(0),
                p2.PontosId(0),
                p3.PontosId(0),
                p4.PontosId(0),
                qntPtos
            );

            ObjetoNovo(spline);

            for (int i = 0; i < spline.GetPontos().Count - 1; i++)
            {
                ObjetoNovo(spline, new SegReta(spline, spline.PontosId(i), spline.PontosId(i + 1)));
                objetoSelecionado.shaderCor = new Shader(
                    "Shaders/shader.vert",
                    "Shaders/shaderAmarela.frag"
                );
            }
            //última reta
            ultimaReta = new SegReta(
                spline,
                spline.PontosId(spline.GetPontos().Count - 1),
                ptosControle[3].PontosId(0)
            );
            ultimaReta.shaderCor = new Shader("Shaders/shader.vert", "Shaders/shaderAmarela.frag");
            ObjetoNovo(spline, ultimaReta);


            s1 = new SegReta(null, p1.PontosId(0), p2.PontosId(0));
            s2 = new SegReta(null, p2.PontosId(0), p3.PontosId(0));
            s3 = new SegReta(null, p3.PontosId(0), p4.PontosId(0));
            s1.shaderCor = new Shader("Shaders/shader.vert", "Shaders/shaderCiano.frag");
            s2.shaderCor = new Shader("Shaders/shader.vert", "Shaders/shaderCiano.frag");
            s3.shaderCor = new Shader("Shaders/shader.vert", "Shaders/shaderCiano.frag");
            ObjetoNovo(s1);
            ObjetoNovo(s2);
            ObjetoNovo(s3);

            objetoSelecionado = p1;
            objetoSelecionado.ObjetoAtualizar();

#if CG_Privado
            #region Objeto: circulo
            objetoNovo = new Circulo(null, 0.2, new Ponto4D());
            objetoNovo.shaderCor = new Shader("Shaders/shader.vert", "Shaders/shaderAmarela.frag");
            ObjetoNovo(objetoNovo);
            objetoNovo = null;
            #endregion

            #region Objeto: SrPalito
            objetoNovo = new SrPalito(null);
            ObjetoNovo(objetoNovo);
            objetoNovo = null;
            SrPalito objSrPalito = objetoSelecionado as SrPalito;
            #endregion

            #region Objeto: Spline
            objetoNovo = new Spline(null);
            ObjetoNovo(objetoNovo);
            objetoNovo = null;
            Spline objSpline = objetoSelecionado as Spline;
            #endregion
#endif
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);

            GL.Clear(ClearBufferMask.ColorBufferBit);

#if CG_Gizmo
            Sru3D();
#endif
            for (var i = 0; i < objetosLista.Count; i++)
                objetosLista[i].Desenhar();

            SwapBuffers();
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);

            #region Teclado
            var input = KeyboardState;
            if (input.IsKeyDown(Keys.Escape))
            {
                Close();
            }
            else
            {
              if (input.IsKeyDown(Keys.R)) {
                //reseta
                ptosControle[0].PontosAlterar(new Ponto4D(-0.5, -0.5), 0);
                ptosControle[1].PontosAlterar(new Ponto4D(-0.5, 0.5), 0);
                ptosControle[2].PontosAlterar(new Ponto4D(0.5, 0.5), 0);
                ptosControle[3].PontosAlterar(new Ponto4D(0.5, -0.5), 0);
                
                ptosControle[0].ObjetoAtualizar();
                ptosControle[1].ObjetoAtualizar();
                ptosControle[2].ObjetoAtualizar();
                ptosControle[3].ObjetoAtualizar();


                s1.PontosAlterar(p1.PontosId(0), 0);
                s1.PontosAlterar(p2.PontosId(0), 1);
                s1.ObjetoAtualizar();

                s2.PontosAlterar(p2.PontosId(0), 0);
                s2.PontosAlterar(p3.PontosId(0), 1);
                s2.ObjetoAtualizar();

                s3.PontosAlterar(p3.PontosId(0), 0);
                s3.PontosAlterar(p4.PontosId(0), 1);
                s3.ObjetoAtualizar();
            

                AtualizarSpline();
              }
              else if (input.IsKeyPressed(Keys.Minus))
              {
                if (qntPtos > 4) {
                    RemoverPontoSpline();
                }
              }
              else if (input.IsKeyPressed(Keys.KeyPadAdd))
              {
                AdicionarPontoSpline();
              }
              else if (input.IsKeyPressed(Keys.Equal))
              {
                AdicionarPontoSpline();
              }
              else if (input.IsKeyPressed(Keys.C))
              {
                  Ponto4D pto = new Ponto4D(
                      objetoSelecionado.PontosId(0).X,
                      objetoSelecionado.PontosId(0).Y + 0.05f
                  );
                  objetoSelecionado.PontosAlterar(pto, 0);
                  objetoSelecionado.ObjetoAtualizar();
                  getSegReta(contador, pto);

                  AtualizarSpline();
              }
              else if (input.IsKeyPressed(Keys.B))
              {
                  Ponto4D pto = new Ponto4D(
                      objetoSelecionado.PontosId(0).X,
                      objetoSelecionado.PontosId(0).Y - 0.05f
                  );
                  objetoSelecionado.PontosAlterar(pto, 0);
                  objetoSelecionado.ObjetoAtualizar();
                  getSegReta(contador, pto);

                  AtualizarSpline();
              }
              else if (input.IsKeyPressed(Keys.E))
              {
                  Ponto4D pto = new Ponto4D(
                      objetoSelecionado.PontosId(0).X - 0.05f,
                      objetoSelecionado.PontosId(0).Y
                  );
                  objetoSelecionado.PontosAlterar(pto, 0);
                  objetoSelecionado.ObjetoAtualizar();
                  getSegReta(contador, pto);

                  AtualizarSpline();
              }
              else if (input.IsKeyPressed(Keys.D))
              {
                  Ponto4D pto = new Ponto4D(
                      objetoSelecionado.PontosId(0).X + 0.05f,
                      objetoSelecionado.PontosId(0).Y
                  );
                  objetoSelecionado.PontosAlterar(pto, 0);
                  objetoSelecionado.ObjetoAtualizar();
                  getSegReta(contador, pto);

                  AtualizarSpline();
              }
              else
              {
                  if (input.IsKeyPressed(Keys.Space))
                  {
                      p1.shaderCor = new Shader(
                          "Shaders/shader.vert",
                          "Shaders/shaderBranca.frag"
                      );
                      p2.shaderCor = new Shader(
                          "Shaders/shader.vert",
                          "Shaders/shaderBranca.frag"
                      );
                      p3.shaderCor = new Shader(
                          "Shaders/shader.vert",
                          "Shaders/shaderBranca.frag"
                      );
                      p4.shaderCor = new Shader(
                          "Shaders/shader.vert",
                          "Shaders/shaderBranca.frag"
                      );
                      switch (contador)
                      {
                          case 0:
                              p2.shaderCor = _shaderVermelha;
                              objetoSelecionado = p1;
                              break;
                          case 1:
                              tamanhoLista = 2;
                              p3.shaderCor = _shaderVermelha;
                              objetoSelecionado = p2;
                              break;
                          case 2:
                              tamanhoLista = 2;
                              p4.shaderCor = _shaderVermelha;
                              objetoSelecionado = p3;
                              break;
                          case 3:
                              tamanhoLista = 1;
                              p1.shaderCor = _shaderVermelha;
                              objetoSelecionado = p4;
                              break;
                          default:
                              break;
                      }
                      contador++;
                      if (contador > 3)
                      {
                          contador = 0;
                      }
                      objetoSelecionado.ObjetoAtualizar();

                      if (objetoSelecionado == null)
                          Console.WriteLine("objetoSelecionado: NULL!");
                      else if (objetosLista.Count == 0)
                          Console.WriteLine("objetoLista: vazia!");
                      else
                      {
                          int ind = 0;
                          foreach (var objetoNovo in objetosLista)
                          {
                              if (objetoNovo == objetoSelecionado)
                              {
                                  ind++;
                                  if (ind >= objetosLista.Count)
                                      ind = 0;
                                  break;
                              }
                              ind++;
                          }
                          objetoSelecionado = objetosLista[ind];
                      }
                  }
                  else
                  {
                      if (input.IsKeyPressed(Keys.C))
                      {
                          objetoSelecionado.shaderCor = new Shader(
                              "Shaders/shader.vert",
                              "Shaders/shaderCiano.frag"
                          );
                      }
                  }
              }
            }
            #endregion

            #region  Mouse
            var mouse = MouseState;
            // Mouse FIXME: inverte eixo Y, fazer NDC para proporção em tela
            Vector2i janela = this.ClientRectangle.Size;

            if (input.IsKeyDown(Keys.LeftShift))
            {
                if (_firstMove)
                {
                    _lastPos = new Vector2(mouse.X, mouse.Y);
                    _firstMove = false;
                }
                else
                {
                    var deltaX = (mouse.X - _lastPos.X) / janela.X;
                    var deltaY = (mouse.Y - _lastPos.Y) / janela.Y;
                    _lastPos = new Vector2(mouse.X, mouse.Y);

                    objetoSelecionado.PontosAlterar(
                        new Ponto4D(
                            objetoSelecionado.PontosId(0).X + deltaX,
                            objetoSelecionado.PontosId(0).Y + deltaY,
                            0
                        ),
                        0
                    );
                    objetoSelecionado.ObjetoAtualizar();
                }
            }
            if (input.IsKeyDown(Keys.RightShift))
            {
                objetoSelecionado.PontosAlterar(
                    new Ponto4D(mouse.X / janela.X, mouse.Y / janela.Y, 0),
                    0
                );
                objetoSelecionado.ObjetoAtualizar();
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
            foreach (var objeto in objetosLista)
            {
                objeto.OnUnload();
            }

            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindVertexArray(0);
            GL.UseProgram(0);

            GL.DeleteBuffer(_vertexBufferObject_sruEixos);
            GL.DeleteVertexArray(_vertexArrayObject_sruEixos);

            GL.DeleteProgram(_shaderVermelha.Handle);
            GL.DeleteProgram(_shaderVerde.Handle);
            GL.DeleteProgram(_shaderAzul.Handle);

            base.OnUnload();
        }

#if CG_Gizmo
        private void Sru3D()
        {
#if CG_OpenGL && !CG_DirectX
            GL.BindVertexArray(_vertexArrayObject_sruEixos);
            // EixoX
            _shaderVermelha.Use();
            GL.DrawArrays(PrimitiveType.Lines, 0, 2);
            // EixoY
            _shaderVerde.Use();
            GL.DrawArrays(PrimitiveType.Lines, 2, 2);
            // EixoZ
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
