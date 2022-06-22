using CG_Biblioteca;
using OpenTK.Graphics.OpenGL;

namespace gcgcg
{
    internal class SegReta : ObjetoGeometria
    {
        public SegReta(char rotulo, Objeto paiRef, Ponto4D pontoA, Ponto4D pontoB) : base(rotulo, paiRef)
        {
            PrimitivaTipo = PrimitiveType.Lines;
            PontosAdicionar(pontoA);
            PontosAdicionar(pontoB);
        }

        protected override void DesenharObjeto()
        {
            GL.Begin(PrimitivaTipo);
            foreach (var ponto in pontosLista)
            {
                GL.Vertex3(ponto.X, ponto.Y, ponto.Z);
            }

            GL.End();
        }
    }
}