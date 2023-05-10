#define CG_Debug

using CG_Biblioteca;
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
    public int pontosReta;

    public Spline(Objeto paiRef, Ponto4D ptoA, Ponto4D ptoB, Ponto ptoC, Ponto4D ptoD, int pontosReta) : base(paiRef)
    {
      this.ptoA = ptoA;
      this.ptoB = ptoB;
      this.ptoC = ptoC;
      this.ptoD = ptoD;
    }

    public drawSpline(){
        // rever logica do método de desenhar spline
        // envolve as metadinhas dos segmentos pra achar X e Y
        // rever certinho em qual metadinha x e y vão receber
    }

  }
}
