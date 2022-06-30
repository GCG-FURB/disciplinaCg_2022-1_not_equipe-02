using CG_Biblioteca;
using OpenTK.Graphics.OpenGL;

namespace CG_N4
{
    public class Chao : ObjetoGeometria
    {
        public readonly Ponto4D Centro;
        public readonly double Tamanho;

        public Chao(Ponto4D centro, double tamanho) : base(Utilitario.charProximo(), null)
        {
            PrimitivaTipo = PrimitiveType.Quads;
            Textura = new Textura("/home/ariel/Downloads/_.jpeg");
            Centro = centro;
            Tamanho = tamanho;
        }

        protected override void DesenharGeometria()
        {
            double x = Tamanho / 2;

            GL.Begin(PrimitivaTipo);

            GL.Normal3(0.0f, 1.0f, 0.0f);

            GL.TexCoord2(0.0f, 30.0f);
            GL.Vertex3(Centro.X - x, Centro.Y, Centro.Z + x);

            GL.TexCoord2(30.0f, 30.0f);
            GL.Vertex3(Centro.X + x, Centro.Y, Centro.Z + x);

            GL.TexCoord2(30.0f, 0.0f);
            GL.Vertex3(Centro.X + x, Centro.Y, Centro.Z - x);

            GL.TexCoord2(0.0f, 0.0f);
            GL.Vertex3(Centro.X - x, Centro.Y, Centro.Z - x);

            GL.End();
        }
    }
}