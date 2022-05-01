using CG_Biblioteca;
using OpenTK.Graphics.OpenGL;

namespace gcgcg
{
    internal class Spline : Objeto
    {
        public SegReta LinhaEsquerda { get; private set; }
        public SegReta LinhaLigacao { get; private set; }
        public SegReta LinhaDireita { get; private set; }

        public Spline(char rotulo, Objeto paiRef, SegReta linhaEsquerda, SegReta linhaLigacao, SegReta linhaDireita) : base(rotulo, paiRef)
        {
            PrimitivaTipo = PrimitiveType.Lines;
            LinhaEsquerda = linhaEsquerda;
            LinhaLigacao = linhaLigacao;
            LinhaDireita = linhaDireita;
        }

        protected override void DesenharGeometria()
        {
            // 2 pontos por cada segReta - 1 seg reta esquerda, 1 direita, 1 de ligação
            var quantidadeDePontos = 6d;
            var incremento = 1d / quantidadeDePontos;
            var pontoReferencia = LinhaEsquerda.PontoA;
            for (double t = 0; t <= 1; t+= incremento )
            {
                var p1p2 = Calcular(LinhaEsquerda.PontoA, LinhaLigacao.PontoA, t);
                var p2p3 = Calcular(LinhaLigacao.PontoA, LinhaDireita.PontoA, t);
                var p3p4 = Calcular(LinhaDireita.PontoA, LinhaDireita.PontoB, t);

                var p1p2p3 = Calcular(p1p2, p2p3, t);
                var p2p3p4 = Calcular(p2p3, p3p4, t);

                var p1p2p3p4 = Calcular(p1p2p3, p2p3p4, t);


                GL.Begin(PrimitivaTipo);
                GL.Vertex2(pontoReferencia.X, pontoReferencia.Y);
                GL.Vertex2(p1p2p3p4.X, p1p2p3p4.Y);
                GL.End();

                pontoReferencia = p1p2p3p4;
            }
        }

        private Ponto4D Calcular(Ponto4D pontoA, Ponto4D pontoB, double t)
        {
            var ponto4dParaDesenharSpline = new Ponto4D();
            ponto4dParaDesenharSpline.X = pontoA.X + ((pontoB.X - pontoA.X) * t);
            ponto4dParaDesenharSpline.Y = pontoA.Y + ((pontoB.Y - pontoA.Y) * t);
            return ponto4dParaDesenharSpline;
        }
    }
}