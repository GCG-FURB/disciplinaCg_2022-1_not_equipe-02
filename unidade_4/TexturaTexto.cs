using System;
using System.Drawing;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace CG_N4
{
    public class TexturaTexto : Textura
    {
        private static readonly Font Serif = new Font(FontFamily.GenericSerif, 14);

        public readonly float Width;
        public readonly float Height;

        public TexturaTexto(string texto, int width, int height)
        {
            if (width <= 0)
                throw new ArgumentOutOfRangeException("width");
            if (height <= 0)
                throw new ArgumentOutOfRangeException("height ");
            if (GraphicsContext.CurrentContext == null)
                throw new InvalidOperationException("No GraphicsContext is current on the calling thread.");

            Bitmap bmp = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            Graphics gfx = Graphics.FromImage(bmp);
            gfx.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;

            gfx.Clear(Color.Black);

            PointF position = PointF.Empty;
            gfx.DrawString(texto, Serif, Brushes.White, position);

            SizeF size = gfx.MeasureString(texto, Serif);
            Rectangle dirtyRegion = Rectangle.Round(new RectangleF(position, size));

            // crop it!
            Bitmap bitmap = bmp.Clone(dirtyRegion, System.Drawing.Imaging.PixelFormat.DontCare);

            System.Drawing.Imaging.BitmapData data = bitmap.LockBits(
                dirtyRegion,
                System.Drawing.Imaging.ImageLockMode.ReadOnly,
                System.Drawing.Imaging.PixelFormat.Format32bppArgb
            );
            
            GL.GenTextures(1, out Id);
            GL.BindTexture(TextureTarget.Texture2D, Id);
            
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int) TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int) TextureMagFilter.Linear);

            GL.TexImage2D(
                TextureTarget.Texture2D,
                0,
                PixelInternalFormat.Rgba,
                dirtyRegion.Width,
                dirtyRegion.Height,
                0,
                PixelFormat.Rgba,
                PixelType.UnsignedByte,
                IntPtr.Zero
            );

            GL.TexSubImage2D(
                TextureTarget.Texture2D,
                0,
                dirtyRegion.X,
                dirtyRegion.Y,
                dirtyRegion.Width,
                dirtyRegion.Height,
                PixelFormat.Bgra,
                PixelType.UnsignedByte,
                data.Scan0
            );
            GL.BindTexture(TextureTarget.Texture2D, 0);
            bitmap.UnlockBits(data);
            bitmap.Dispose();
            bmp.Dispose();
            gfx.Dispose();

            Width = size.Width;
            Height = size.Height;
        }
    }
}