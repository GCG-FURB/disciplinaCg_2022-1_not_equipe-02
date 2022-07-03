using System;
using System.Linq;
using OpenTK.Graphics.OpenGL;

namespace CG_N4
{
    public class Texto2D : Objeto
    {

        public readonly string Texto;
        public float Width;
        public float Height;

        public Texto2D(string texto) : base(Utilitario.charProximo(), null)
        {
            Texto = texto;
            TexturaTexto texturaTexto = new TexturaTexto(texto, 800, 800);
            Textura = texturaTexto;
            Width = texturaTexto.Width;
            Height = texturaTexto.Height;
            PrimitivaTipo = PrimitiveType.Quads;            
        }

        protected override void DesenharGeometria()
        {
            GL.Begin(PrimitivaTipo);
            
            GL.Normal3(-1, 0, 0);
            GL.TexCoord2(0.0f, 1.0f); GL.Vertex2(0f, 0f);
            GL.TexCoord2(1.0f, 1.0f); GL.Vertex2(Width, 0f);
            GL.TexCoord2(1.0f, 0.0f); GL.Vertex2(Width, Height);
            GL.TexCoord2(0.0f, 0.0f); GL.Vertex2(0f, Height);
            GL.End();
        }

        public void Dispose()
        {
            Textura.Dispose();
        }
    }
}