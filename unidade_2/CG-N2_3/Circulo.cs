using CG_Biblioteca;
using OpenTK.Graphics.OpenGL;

namespace gcgcg
{
    internal class Circulo : Objeto
    {
        private readonly int pontos;
        private readonly int raio;

        public Circulo(char rotulo, Objeto paiRef, int pontos, int raio) : base(rotulo, paiRef)
        {
            PrimitivaTipo = PrimitiveType.Points;
            this.pontos = pontos;
            this.raio = raio;
        }

        protected override void DesenharGeometria()
        {
            GL.Begin(PrimitivaTipo);

            var anguloPonto = 360 / pontos;
            for (var i = 0; i < pontos; i++)
            {
                var ponto = Matematica.GerarPtosCirculo(anguloPonto * i, raio);
                GL.Vertex2(ponto.X, ponto.Y);
            }

            GL.End();
        }
    }
}