using CG_Biblioteca;
using OpenTK.Graphics.OpenGL;

namespace gcgcg
{
    public class Poligono : ObjetoGeometria
    {
        public Poligono(char rotulo, Objeto paiRef) : base(rotulo, paiRef)
        {
            PrimitivaTipo = PrimitiveType.LineLoop;
            PrimitivaTamanho = 3;
            ObjetoCor = new Cor(255, 255, 255);
        }

        protected override void DesenharObjeto()
        {
            GL.Begin(PrimitivaTipo);

            foreach (var ponto in pontosLista)
            {
                GL.Vertex2(ponto.X, ponto.Y);
            }
            
            GL.End();
        }

        public void ModificarCoordenadaUltimoPonto(int mouseX, int mouseY)
        {
            PontosUltimo().X = mouseX;
            PontosUltimo().Y = mouseY;
        }
    }
}