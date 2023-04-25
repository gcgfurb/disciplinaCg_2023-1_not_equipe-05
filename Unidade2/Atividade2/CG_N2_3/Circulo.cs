#define CG_Debug // debugar texto.
using CG_Biblioteca;
using OpenTK.Graphics.OpenGL4;

namespace gcgcg
{
    internal class Circulo : Objeto
    {
        public Circulo(Objeto paiRef, double tam) : base(paiRef)
        {
            PrimitivaTipo = PrimitiveType.Quads;
            Ponto4D pto = new Ponto4D();
            for (int i=0; i<360; i+=5){
                pto = Matematica.GerarPtosCirculo(i, tam);
                base.PontosAdicionar(pto);
            }
            base.ObjetoAtualizar();        
        }

        #if CG_Debug
        public override string ToString()
        {
        string retorno;
        retorno = "__ Objeto Retangulo _ Tipo: " + PrimitivaTipo + "\n";
        retorno += base.ToString();
        return (retorno);

        }
        #endif
    }
}