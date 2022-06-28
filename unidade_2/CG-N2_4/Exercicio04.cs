using CG_Biblioteca;
using OpenTK.Graphics.OpenGL;

namespace gcgcg
{
    internal class Exercicio04 : Objeto
    {
        private Ponto4D PontoA;
        private Ponto4D PontoB;
        private Ponto4D PontoC;
        private Ponto4D PontoD;
        
        public Exercicio04(char rotulo, Objeto paiRef, PrimitiveType tipoPrimitiva, Ponto4D pontoA, Ponto4D pontoB, Ponto4D pontoC, Ponto4D pontoD) : base(rotulo, paiRef)
        {
            PontoA = pontoA;
            PontoB = pontoB;
            PontoC = pontoC;
            PontoD = pontoD;
            PrimitivaTamanho = 10;
            PrimitivaTipo = tipoPrimitiva;
        }

        protected override void DesenharGeometria()
        {
            GL.Begin(PrimitivaTipo);
            GL.Color3(1.0f,0.0f,0.0f);
            GL.Vertex2(PontoA.X, PontoA.Y);
            GL.Color3(1.0f,0.0f,1.0f);
            GL.Vertex2(PontoB.X, PontoB.Y);
            GL.Color3(1.0f,0.0f,0.0f);
            GL.Vertex2(PontoC.X, PontoC.Y);
            GL.Color3(1.0f,0.0f,1.0f);
            GL.Vertex2(PontoD.X, PontoD.Y);
            
            GL.End();
        }
    }
}