using CG_Biblioteca;
using OpenTK.Graphics.OpenGL;

namespace CG_N4
{
    internal class SegReta : ObjetoGeometria
    {
        public SegReta(char rotulo, Objeto paiRef, Ponto4D pontoA, Ponto4D pontoB) : base(rotulo, paiRef)
        {
            PrimitivaTipo = PrimitiveType.Lines;
            PontosAdicionar(pontoA);
            PontosAdicionar(pontoB);
        }
    }
}