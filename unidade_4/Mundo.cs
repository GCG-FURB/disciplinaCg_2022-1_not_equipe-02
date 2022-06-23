/**
  Autor: Dalton Solano dos Reis
**/

using System;
using System.Collections.Generic;
using CG_Biblioteca;
using CG_N3;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;

namespace gcgcg
{
    class Mundo : GameWindow
    {
        private static Mundo instanciaMundo;

        private Mundo(int width, int height) : base(width, height)
        {
        }

        public static Mundo GetInstance(int width, int height)
        {
            if (instanciaMundo == null)
            {
                instanciaMundo = new Mundo(width, height);
            }

            return instanciaMundo;
        }

        private CameraPerspective camera = new CameraPerspective();
        private List<Objeto> objetosLista = new List<Objeto>();
        private ObjetoGeometria objetoSelecionado = null;

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            camera.Eye = new Vector3(
                (float)Utilitario.MetrosEmPixels(-5.0),
                (float)Utilitario.MetrosEmPixels(1.5),
                (float)Utilitario.MetrosEmPixels(1.25)
            );
            camera.At = new Vector3(
                (float)Utilitario.MetrosEmPixels(0.0),
                (float)Utilitario.MetrosEmPixels(1.5),
                (float)Utilitario.MetrosEmPixels(1.25)
            );

            camera.Aspect = Width / (float)Height;
            camera.Far = 2000;

            objetosLista.Add(new Cancha(
                new Ponto4D(),
                Utilitario.MetrosEmPixels(2.5),
                Utilitario.MetrosEmPixels(23.0),
                Utilitario.MetrosEmPixels(1.0)
            ));

            Console.WriteLine(" --- Ajuda / Teclas: ");
            Console.WriteLine(" [  H     ] mostra teclas usadas. ");

            GL.ClearColor(0.5f, 0.5f, 0.5f, 1.0f);
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);
            DefinirProjecaoCamera();
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            Matrix4 modelview = Matrix4.LookAt(camera.Eye, camera.At, camera.Up);
            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadMatrix(ref modelview);

            Sru3D();

            foreach (var objeto in objetosLista)
            {
                objeto.Desenhar();
            }

            objetoSelecionado?.BBox.Desenhar();

            SwapBuffers();
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            DefinirProjecaoCamera();
        }

        protected override void OnMouseMove(MouseMoveEventArgs e)
        {
            MouseCG.X = e.X;
            MouseCG.Y = Height - e.Y;
        }

        protected override void OnKeyDown(KeyboardKeyEventArgs e)
        {
            if (e.Key == Key.H)
            {
                Utilitario.AjudaTeclado();
            }
            else
            {
                Console.WriteLine(" __ Tecla não implementada.");
            }
        }

        private void DefinirProjecaoCamera()
        {
            GL.Viewport(ClientRectangle.X, ClientRectangle.Y, ClientRectangle.Width, ClientRectangle.Height);
            Matrix4 projection = Matrix4.CreatePerspectiveFieldOfView(camera.Fovy, camera.Aspect, camera.Near, camera.Far);
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadMatrix(ref projection);
        }

        private void Sru3D()
        {
            GL.LineWidth(1);
            GL.Begin(PrimitiveType.Lines);
            GL.Color3(Convert.ToByte(255), Convert.ToByte(0), Convert.ToByte(0));
            GL.Vertex3(0, 0, 0);
            GL.Vertex3(200, 0, 0);
            GL.Color3(Convert.ToByte(0), Convert.ToByte(255), Convert.ToByte(0));
            GL.Vertex3(0, 0, 0);
            GL.Vertex3(0, 200, 0);
            GL.Color3(Convert.ToByte(0), Convert.ToByte(0), Convert.ToByte(255));
            GL.Vertex3(0, 0, 0);
            GL.Vertex3(0, 0, 200);
            GL.End();
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            ToolkitOptions.Default.EnableHighResolution = false;

            Mundo window = Mundo.GetInstance(1280, 720);
            window.Title = "CG_N4";
            window.Run(1.0 / 60.0);
        }
    }
}