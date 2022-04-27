using CG_Biblioteca;
using OpenTK.Graphics.OpenGL;

namespace gcgcg
{
    internal class Linha: Objeto
    {
        private readonly Ponto4D PontoA;
        private readonly Ponto4D PontoB;
        
        public Linha(char rotulo, Objeto paiRef, Ponto4D pontoA, Ponto4D pontoB) : base(rotulo, paiRef)
        {
            PrimitivaTipo = PrimitiveType.Lines;
            PontoA = pontoA;
            PontoB = pontoB;
        }

        protected override void DesenharGeometria()
        {
            GL.Begin(PrimitivaTipo);
            
            GL.Vertex2(PontoA.X, PontoA.Y);
            GL.Vertex2(PontoB.X, PontoB.Y);
            
            GL.End();
        }
    }
}