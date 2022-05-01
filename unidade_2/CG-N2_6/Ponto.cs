using CG_Biblioteca;
using OpenTK.Graphics.OpenGL;

namespace gcgcg
{
    public class Ponto : Objeto
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
    }
}