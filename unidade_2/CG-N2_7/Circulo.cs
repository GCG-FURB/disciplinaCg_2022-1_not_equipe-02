using CG_Biblioteca;
using OpenTK.Graphics.OpenGL;

namespace gcgcg
{
    internal class Circulo : Objeto
    {
        private readonly int pontos;
        private readonly int raio;

        public readonly Ponto4D Centro;

        public Circulo(char rotulo, Objeto paiRef, int pontos, int raio) : base(rotulo, paiRef)
        {
            PrimitivaTipo = PrimitiveType.LineLoop;
            this.pontos = pontos;
            this.raio = raio;
        }
        
        public Circulo(char rotulo, Objeto paiRef, int pontos, int raio, Ponto4D ptoCentro) : base(rotulo, paiRef)
        {
            PrimitivaTipo = PrimitiveType.LineLoop;
            this.pontos = pontos;
            this.raio = raio;
            this.Centro = ptoCentro;

            var pontoCE = Matematica.GerarPtosCirculo(45, raio) + ptoCentro;
            var pontoBD = Matematica.GerarPtosCirculo(225, raio) + ptoCentro;
            this.BBox.Atribuir(pontoCE);
            this.BBox.Atualizar(pontoBD);
        }

        protected override void DesenharGeometria()
        {
            GL.Begin(PrimitivaTipo);

            var anguloPonto = 360 / pontos;
            for (var i = 0; i < pontos; i++)
            {
                var ponto = Matematica.GerarPtosCirculo(anguloPonto * i, raio);
                if (Centro != null)
                {
                    var pontoResultante = Centro + ponto;
                    GL.Vertex2(pontoResultante.X, pontoResultante.Y);
                }
                else
                {
                    GL.Vertex2(ponto.X, ponto.Y);
                }
            }

            GL.End();
        }
    }
}