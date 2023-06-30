#define CG_Debug

using CG_Biblioteca;
using OpenTK.Graphics.OpenGL4;

namespace gcgcg
{
  internal class Retangulo : Objeto
  {
    public Retangulo(Objeto paiRef, ref char _rotulo, Ponto4D ptoInfEsq, Ponto4D ptoSupDir, bool changeAxis) : base(paiRef, ref _rotulo)  
    {
      PrimitivaTipo = PrimitiveType.TriangleFan;
      PrimitivaTamanho = 10;
      if (changeAxis == true){
        base.PontosAdicionar(ptoInfEsq);
        base.PontosAdicionar(new Ponto4D(ptoSupDir.X, ptoInfEsq.Y, ptoSupDir.Z));
        base.PontosAdicionar(ptoSupDir);
        base.PontosAdicionar(new Ponto4D(ptoInfEsq.X, ptoSupDir.Y, ptoInfEsq.Z));
        Atualizar();
      }
      if (changeAxis == false){
        base.PontosAdicionar(ptoInfEsq);
        base.PontosAdicionar(new Ponto4D(ptoSupDir.X, ptoInfEsq.Y, ptoInfEsq.Z));
        base.PontosAdicionar(ptoSupDir);
        base.PontosAdicionar(new Ponto4D(ptoInfEsq.X, ptoSupDir.Y, ptoSupDir.Z));
        Atualizar();
      }
    }

    public void Atualizar()
    {

      base.ObjetoAtualizar();
    }

#if CG_Debug
    public override string ToString()
    {
      string retorno;
      retorno  = "__ Objeto Retangulo _ Tipo: " + PrimitivaTipo + " _ Tamanho: " + PrimitivaTamanho + "\n";
      retorno += base.ImprimeToString();
      return (retorno);
    }
#endif

  }
}
