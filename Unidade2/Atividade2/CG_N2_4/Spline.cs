#define CG_Debug

using CG_Biblioteca;
using System.Collections.Generic;
using OpenTK.Graphics.OpenGL4;

namespace gcgcg
{
  internal class Spline : Objeto
  {

    //precisa adicionar paiRef será?
    private Ponto4D ptoA;
    private Ponto4D ptoB;
    private Ponto4D ptoC;
    private Ponto4D ptoD;
    public int qntPtos;
    public List<Ponto4D> ptosSpline = new List<Ponto4D>();

    public Spline(Objeto paiRef, Ponto4D ptoA, Ponto4D ptoB, Ponto4D ptoC, Ponto4D ptoD, int qntPtos) : base(paiRef)
    {
      this.ptoA = ptoA;
      this.ptoB = ptoB;
      this.ptoC = ptoC;
      this.ptoD = ptoD;
      this.qntPtos = qntPtos;

      DrawSpline();
    }

    public void DrawSpline(){
      for (int i = 0; i < qntPtos; i++){
        Ponto4D ptoAB = Calcular(ptoA, ptoB);
        Ponto4D ptoBC = Calcular(ptoB, ptoC);
        Ponto4D ptoCD = Calcular(ptoC, ptoD);
        Ponto4D ptoX = Calcular(ptoAB, ptoBC);
        Ponto4D ptoY = Calcular(ptoBC, ptoCD);
        Ponto4D resultado = Calcular(ptoX, ptoY);
        ptosSpline.Add(resultado);
      }
    }

    private Ponto4D Calcular(Ponto4D pto, Ponto4D pto1){
      double X = pto.X + (pto1.X - pto.X);
      double Y = pto.Y + (pto1.Y - pto.Y);

      return new Ponto4D(X, Y);
      }

  }
}
