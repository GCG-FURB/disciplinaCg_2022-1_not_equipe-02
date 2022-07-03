using OpenTK.Graphics.OpenGL;

namespace CG_N4
{
    public class Textura
    {
        protected int Id;

        public void Aplicar()
        {
            GL.BindTexture(TextureTarget.Texture2D, Id);
        }

        public void Remover()
        {
            GL.BindTexture(TextureTarget.Texture2D, 0);
        }

        public void Dispose()
        {
            GL.DeleteTexture(Id);
        }
    }
}