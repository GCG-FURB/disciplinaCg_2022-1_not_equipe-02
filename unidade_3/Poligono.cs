using OpenTK.Graphics.OpenGL;

namespace gcgcg
{
    public class Poligono : ObjetoGeometria
    {
        public Poligono(char rotulo, Objeto paiRef) : base(rotulo, paiRef)
        {
            PrimitivaTipo = PrimitiveType.LineLoop;
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
    }
}