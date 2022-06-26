using CG_Biblioteca;
using OpenTK.Graphics.OpenGL;

namespace CG_N4
{
    internal class Circulo : Objeto
    {
        private readonly int pontos;
        private readonly int raio;
        private readonly Ponto4D ptoCentro;

        public Circulo(char rotulo, Objeto paiRef, int pontos, int raio) : base(rotulo, paiRef)
        {
            PrimitivaTipo = PrimitiveType.Points;
            this.pontos = pontos;
            this.raio = raio;
        }
        
        public Circulo(char rotulo, Objeto paiRef, int pontos, int raio, Ponto4D ptoCentro) : base(rotulo, paiRef)
        {
            PrimitivaTipo = PrimitiveType.Points;
            this.pontos = pontos;
            this.raio = raio;
            this.ptoCentro = ptoCentro;
        }

        protected override void DesenharGeometria()
        {
            GL.Begin(PrimitivaTipo);

            var anguloPonto = 360 / pontos;
            for (var i = 0; i < pontos; i++)
            {
                var ponto = Matematica.GerarPtosCirculo(anguloPonto * i, raio);
                if (ptoCentro != null)
                {
                    var pontoResultante = ptoCentro + ponto;
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