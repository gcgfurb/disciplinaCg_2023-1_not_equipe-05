#define CG_Debug

using CG_Biblioteca;
using System.Collections.Generic;
using OpenTK.Graphics.OpenGL4;

namespace gcgcg
{
  internal class Spline : Objeto
  {
    public int qntPtos;
    public List<Ponto4D> ptosSpline = new List<Ponto4D>();

    public Spline(Objeto paiRef, Ponto4D ptoA, Ponto4D ptoB, Ponto4D ptoC, Ponto4D ptoD, int qntPtos) : base(paiRef)
    {
      PrimitivaTipo = PrimitiveType.LineLoop;
      PrimitivaTamanho = 1;
      base.PontosAdicionar(ptoA);
      base.PontosAdicionar(ptoB);
      base.PontosAdicionar(ptoC);
      base.PontosAdicionar(ptoD);
      this.qntPtos = qntPtos;

      Atualizar();
      DrawSpline();
    }
    public void Atualizar()
    {

      base.ObjetoAtualizar();
    }

    public void DrawSpline()
    {
      for (int i = 0; i < qntPtos; i++){
        Ponto4D ptoAB = Calcular(this.PontosId(0), this.PontosId(1));
        Ponto4D ptoBC = Calcular(this.PontosId(1), this.PontosId(2));
        Ponto4D ptoCD = Calcular(this.PontosId(2), this.PontosId(3));
        Ponto4D ptoX = Calcular(ptoAB, ptoBC);
        Ponto4D ptoY = Calcular(ptoBC, ptoCD);
        Ponto4D resultado = Calcular(ptoX, ptoY);
        Atualizar();
        ptosSpline.Add(resultado);
      }
    }

    private Ponto4D Calcular(Ponto4D pto, Ponto4D pto1)
    {
      double X = pto.X + (pto1.X - pto.X);
      double Y = pto.Y + (pto1.Y - pto.Y);

      return new Ponto4D(X, Y);
    }

  }
}
