/**
  Autor: Dalton Solano dos Reis
**/

using System;
using System.Collections.Generic;
using CG_Biblioteca;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;

namespace CG_N4
{
    class Mundo : GameWindow
    {
        private static Mundo _instancia;

        private const float MouseSensibility = 0.05f;
        private const float CameraSpeed = 100.0f;

        private readonly CameraPerspective Camera = new CameraPerspective();
        private List<Objeto> Objetos = new List<Objeto>();
        private Objeto ObjetoSelecionado = null;

        private Mundo(int width, int height) : base(width, height)
        {
        }

        public static Mundo GetInstance()
        {
            if (_instancia == null)
            {
                _instancia = new Mundo(1280, 720);
            }

            return _instancia;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            GL.Enable(EnableCap.DepthTest);
            GL.Enable(EnableCap.CullFace);

            Camera.Eye = new Vector3(
                (float)Utilitario.MetrosEmPixels(-2.0),
                (float)Utilitario.MetrosEmPixels(1.5),
                (float)Utilitario.MetrosEmPixels(1.25)
            );
            Camera.At = new Vector3(1.0f, 0.0f, 0.0f);
            Camera.Aspect = Width / (float)Height;
            Camera.Far = 3000;

            Objetos.Add(new Cancha(
                new Ponto4D(),
                Utilitario.MetrosEmPixels(2.5),
                Utilitario.MetrosEmPixels(23.0),
                Utilitario.MetrosEmPixels(1.0)
            ));

            var esfera = new EsferaTeste(10, new Vector3(1, 0, 0));
            esfera.ObjetoCor = new Cor(255, 0, 0);
            esfera.Translacao(0, 10, 125);
            Objetos.Add(esfera);

            esfera = new EsferaTeste(10, new Vector3(0.8f, 0, 0.2f));
            esfera.ObjetoCor = new Cor(0, 255, 0);
            esfera.Translacao(0, 10, 125);
            Objetos.Add(esfera);

            // ObjetoSelecionado = esfera;

            Console.WriteLine(" --- Ajuda / Teclas: ");
            Console.WriteLine(" [  H     ] mostra teclas usadas. ");

            GL.ClearColor(0.5f, 0.5f, 0.5f, 1.0f);
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);
            CursorVisible = !Focused;
            if (Focused)
            {
                ProcessarCameraTeclado(e);
                ProcessarCameraMouse();
                ProcessarObjetos(e);
            }

            MouseCG.ResetarDelta();
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            Matrix4 modelview = Matrix4.LookAt(Camera.Eye, Camera.Eye + Camera.At, Camera.Up);
            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadMatrix(ref modelview);

            foreach (var objeto in Objetos)
            {
                objeto.Desenhar();
            }

            ObjetoSelecionado?.BBox.Desenhar();

            // Sru3D();
            // Cubo();

            SwapBuffers();
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            DefinirProjecaoCamera();
        }

        protected override void OnMouseMove(MouseMoveEventArgs e)
        {
            if (Focused)
            {
                MouseCG.Atualizar(e.X, e.Y);

                if (!CursorVisible)
                {
                    Point center = new Point(Width / 2, Height / 2);
                    if (e.X == center.X && e.Y == center.Y)
                    {
                        // É o evento do SetPosition, precisa ser ignorado.
                        return;
                    }

                    int deltaX = e.XDelta;
                    int deltaY = e.YDelta;
                    MouseCG.AtualizarDelta(deltaX, deltaY);

                    // Reseta o cursor no centro da tela
                    Point pointToScreen = PointToScreen(center);
                    OpenTK.Input.Mouse.SetPosition(pointToScreen.X, pointToScreen.Y);
                }
            }
        }

        protected override void OnKeyDown(KeyboardKeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.H:
                    Utilitario.AjudaTeclado();
                    break;
            }
        }

        private void ProcessarCameraTeclado(FrameEventArgs e)
        {
            KeyboardState state = Keyboard.GetState();
            if (state.IsKeyDown(Key.W))
            {
                Camera.Eye += Camera.At * CameraSpeed * (float)e.Time;
            }

            if (state.IsKeyDown(Key.S))
            {
                Camera.Eye -= Camera.At * CameraSpeed * (float)e.Time;
            }

            if (state.IsKeyDown(Key.A))
            {
                Camera.Eye -= Vector3.Normalize(Vector3.Cross(Camera.At, Camera.Up)) * CameraSpeed * (float)e.Time;
            }

            if (state.IsKeyDown(Key.D))
            {
                Camera.Eye += Vector3.Normalize(Vector3.Cross(Camera.At, Camera.Up)) * CameraSpeed * (float)e.Time;
            }

            if (state.IsKeyDown(Key.Space))
            {
                Camera.Eye += Camera.Up * CameraSpeed * (float)e.Time;
            }

            if (state.IsKeyDown(Key.LControl))
            {
                Camera.Eye -= Camera.Up * CameraSpeed * (float)e.Time;
            }
        }

        private void ProcessarCameraMouse()
        {
            if (MouseCG.DeltaX != 0 || MouseCG.DeltaY != 0)
            {
                Camera.LookAround(MouseCG.DeltaX * MouseSensibility, MouseCG.DeltaY * MouseSensibility * -1);
            }
        }

        private void ProcessarObjetos(FrameEventArgs e)
        {
            ProcessarColisaoObjetos();
            foreach (Objeto objeto in Objetos)
            {
                objeto.UpdateFrame(e);
            }
        }

        private void ProcessarColisaoObjetos()
        {
            foreach (Objeto objeto in Objetos)
            {
                LimparColisoes(objeto);
            }

            for (int i = 0; i < Objetos.Count; i++)
            {
                Objeto objetoA = Objetos[i];
                for (int x = i + 1; x < Objetos.Count; x++)
                {
                    Objeto objetoB = Objetos[x];
                    ProcessarColisaoObjetos(objetoA, objetoB);
                }
            }
        }

        private void LimparColisoes(Objeto objeto)
        {
            objeto.Colisor?.Colisoes.Clear();
            foreach (Objeto filho in objeto.GetFilhos())
            {
                LimparColisoes(filho);
            }
        }

        private void ProcessarColisaoObjetos(Objeto a, Objeto b)
        {
            if (a.Colisor != null && b.Colisor != null)
            {
                Objeto maiorPrioridade = a.Colisor.Prioridade > b.Colisor.Prioridade ? a : b;
                Objeto menorPrioridade = a.Colisor.Prioridade <= b.Colisor.Prioridade ? a : b;
                if (maiorPrioridade.Colisor.ExisteColisao(menorPrioridade))
                {
                    a.OnColisao(b);
                    b.OnColisao(a);
                    
                    a.Colisor.Colisoes.Add(b);
                    b.Colisor.Colisoes.Add(a);
                }
            }

            IReadOnlyList<Objeto> filhosA = a.GetFilhos();
            foreach (Objeto filhoA in filhosA)
            {
                ProcessarColisaoObjetos(filhoA, b);

                IReadOnlyList<Objeto> filhosB = b.GetFilhos();
                foreach (Objeto filhoB in filhosB)
                {
                    ProcessarColisaoObjetos(filhoA, filhoB);
                }
            }
        }

        private void DefinirProjecaoCamera()
        {
            GL.Viewport(ClientRectangle.X, ClientRectangle.Y, ClientRectangle.Width, ClientRectangle.Height);
            Matrix4 projection = Matrix4.CreatePerspectiveFieldOfView(
                Camera.Fovy,
                Camera.Aspect,
                Camera.Near,
                Camera.Far
            );
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

        private void Cubo()
        {
            GL.Begin(PrimitiveType.Quads);

            // Face da frente
            GL.Color3(0.0f, 0.0f, 1.0f);
            GL.Normal3(0, 0, 1);
            GL.Vertex3(-1.0f, -1.0f, 1.0f);
            GL.Vertex3(1.0f, -1.0f, 1.0f);
            GL.Vertex3(1.0f, 1.0f, 1.0f);
            GL.Vertex3(-1.0f, 1.0f, 1.0f);
            // Face do fundo
            GL.Color3(0.0f, 1.0f, 0.0f);
            GL.Normal3(0, 0, -1);
            GL.Vertex3(-1.0f, -1.0f, -1.0f);
            GL.Vertex3(-1.0f, 1.0f, -1.0f);
            GL.Vertex3(1.0f, 1.0f, -1.0f);
            GL.Vertex3(1.0f, -1.0f, -1.0f);
            // Face de cima
            GL.Color3(1.0f, 0.0f, 0.0f);
            GL.Normal3(0, 1, 0);
            GL.Vertex3(-1.0f, 1.0f, 1.0f);
            GL.Vertex3(1.0f, 1.0f, 1.0f);
            GL.Vertex3(1.0f, 1.0f, -1.0f);
            GL.Vertex3(-1.0f, 1.0f, -1.0f);
            // Face de baixo
            GL.Color3(1.0f, 1.0f, 0.0f);
            GL.Normal3(0, -1, 0);
            GL.Vertex3(-1.0f, -1.0f, 1.0f);
            GL.Vertex3(-1.0f, -1.0f, -1.0f);
            GL.Vertex3(1.0f, -1.0f, -1.0f);
            GL.Vertex3(1.0f, -1.0f, 1.0f);
            // Face da direita
            GL.Color3(0.0f, 1.0f, 1.0f);
            GL.Normal3(1, 0, 0);
            GL.Vertex3(1.0f, -1.0f, 1.0f);
            GL.Vertex3(1.0f, -1.0f, -1.0f);
            GL.Vertex3(1.0f, 1.0f, -1.0f);
            GL.Vertex3(1.0f, 1.0f, 1.0f);
            // Face da esquerda
            GL.Color3(1.0f, 0.0f, 1.0f);
            GL.Normal3(-1, 0, 0);
            GL.Vertex3(-1.0f, -1.0f, 1.0f);
            GL.Vertex3(-1.0f, 1.0f, 1.0f);
            GL.Vertex3(-1.0f, 1.0f, -1.0f);
            GL.Vertex3(-1.0f, -1.0f, -1.0f);

            GL.End();
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            ToolkitOptions.Default.EnableHighResolution = false;

            Mundo window = Mundo.GetInstance();
            window.Title = "CG_N4";
            window.Run(1.0 / 60.0);
        }
    }
}