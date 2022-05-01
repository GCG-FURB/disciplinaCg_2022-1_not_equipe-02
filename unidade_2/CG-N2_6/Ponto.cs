using CG_Biblioteca;
using OpenTK.Graphics.OpenGL;

namespace gcgcg
{
    internal class Ponto : ObjetoGeometria
    {
        public Ponto4D Ponto4D { get; private set; }
        
        public Ponto(char rotulo, Objeto paiRef, Ponto4D ponto4D) : base(rotulo, paiRef)
        {
            PrimitivaTipo = PrimitiveType.Points;
            Ponto4D = ponto4D;
        }

        protected override void DesenharGeometria()
        {
            GL.Begin(PrimitivaTipo);
            
            GL.Vertex2(Ponto4D.X, Ponto4D.Y);
            
            GL.End();
        }

        protected override void DesenharObjeto()
        {
            DesenharGeometria();
        }
        
        
        public void MoverParaEsquerda(uint unidadesParaMover)
        {
            var pontoParaMoverUmaUnidadeParaEsquerda = new Ponto4D(-unidadesParaMover);
            Ponto4D += pontoParaMoverUmaUnidadeParaEsquerda;
        }
        
        public void MoverParaDireita(uint unidadesParaMover)
        {
            var pontoParaMoverUmaUnidadeParaEsquerda = new Ponto4D(unidadesParaMover);
            Ponto4D += pontoParaMoverUmaUnidadeParaEsquerda;
        }
        
        public void MoverParaCima(uint unidadesParaMover)
        {
            var pontoParaMoverUmaUnidadeParaEsquerda = new Ponto4D(0, unidadesParaMover);
            Ponto4D += pontoParaMoverUmaUnidadeParaEsquerda;
        }
        
        public void MoverParaBaixo(uint unidadesParaMover)
        {
            var pontoParaMoverUmaUnidadeParaEsquerda = new Ponto4D(0, -unidadesParaMover);
            Ponto4D += pontoParaMoverUmaUnidadeParaEsquerda;
        }
        
    }
}