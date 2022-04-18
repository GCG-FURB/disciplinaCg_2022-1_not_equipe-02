using CG_Biblioteca;
using OpenTK.Graphics.ES10;
using OpenTK.Graphics.OpenGL;
using GL = OpenTK.Graphics.OpenGL.GL;

namespace gcgcg
{
    internal class SegReta : Objeto
    {
        private Ponto4D PontoA;
        private Ponto4D PontoB;
        
        public SegReta(char rotulo, Objeto paiRef, Ponto4D pontoA, Ponto4D pontoB) : base(rotulo, paiRef)
        {
            PrimitivaTipo = PrimitiveType.Lines;
            PrimitivaTamanho = 5;
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