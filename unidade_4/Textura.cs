using System.Reflection;
using OpenTK.Graphics.OpenGL;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace CG_N4
{
    public class Textura
    {
        private readonly int _id;

        public Textura(Image<Rgba32> image)
        {
            image.Mutate(x => x.Flip(FlipMode.Vertical));

            int i = 0;
            byte[] pixels = new byte[image.Width * image.Height * 4];
            for (var x = 0; x < image.Width; x++)
            {
                for (var y = 0; y < image.Height; y++)
                {
                    Rgba32 pixel = image[x, y];
                    pixels[i++] = pixel.R;
                    pixels[i++] = pixel.G;
                    pixels[i++] = pixel.B;
                    pixels[i++] = pixel.A;
                }
            }

            GL.GenTextures(1, out _id);
            GL.BindTexture(TextureTarget.Texture2D, _id);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);

            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, image.Width, image.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, ref pixels[0]);

            GL.BindTexture(TextureTarget.Texture2D, 0);
        }

        public Textura(string path) : this(Image.Load<Rgba32>(path))
        {
        }

        public static Textura FromResources(string resourceName)
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            Image<Rgba32> image = Image.Load<Rgba32>(assembly.GetManifestResourceStream(resourceName));
            return new Textura(image);
        }

        public void Aplicar()
        {
            GL.BindTexture(TextureTarget.Texture2D, _id);
        }

        public void Remover()
        {
            GL.BindTexture(TextureTarget.Texture2D, 0);
        }
    }
}