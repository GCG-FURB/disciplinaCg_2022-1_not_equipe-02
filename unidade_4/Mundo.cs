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
        private float CameraSpeed = 600.0f;

        private readonly CameraPerspective Camera = new CameraPerspective();
        private List<Objeto> Objetos = new List<Objeto>();
        private List<Texto2D> ObjetosHUD = new List<Texto2D>();
        private Objeto ObjetoSelecionado = null;

        private bool CameraLivre;
        public bool Dalton;
        public bool PodeProcessarObjetos = true;

        public string TextoCentral;

        private Esfera _esfera;

        private Mundo(int width, int height) : base(width, height)
        {
            Atirador.Instance.Camera = Camera;
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
            GL.Enable(EnableCap.Texture2D);
            GL.Hint(HintTarget.PerspectiveCorrectionHint, HintMode.Nicest);
            GL.ClearColor(135.0f / 255.0f, 206.0f / 255.0f, 235.0f / 255.0f, 1.0f);

            // TODO: Somente temporário
            Jogo.Instance.AdicionarJogador(0, "Ariel");
            Jogo.Instance.AdicionarJogador(1, "Eliton");

            Camera.Eye = new Vector3(
                (float) Utilitario.MetrosEmPixels(-0.5),
                (float) Utilitario.MetrosEmPixels(1.0),
                (float) Utilitario.MetrosEmPixels(1.25)
            );
            Camera.At = new Vector3(0.5f, -0.5f, 0.0f);
            Camera.Aspect = Width / (float) Height;
            Camera.Far = (float) Utilitario.MetrosEmPixels(30.0d);

            Objetos.Add(new Chao(
                new Ponto4D(Utilitario.MetrosEmPixels(20.0d), -3d),
                Utilitario.MetrosEmPixels(60))
            );
            Objetos.Add(new Cancha(
                new Ponto4D(),
                Utilitario.MetrosEmPixels(2.5),
                Utilitario.MetrosEmPixels(26.0),
                Utilitario.MetrosEmPixels(1.5)
            ));

            _esfera = BolaFactory.BuildBocha(Jogo.Instance.Times[1]);
            Objetos.Add(_esfera);

            Jogo.Instance.Iniciar();
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);
            CursorVisible = !Focused || !CameraLivre;
            if (Focused)
            {
                if (CameraLivre)
                {
                    ProcessarCameraTeclado(e);
                    ProcessarCameraMouse();
                }

                if (PodeProcessarObjetos)
                {
                    Atirador.Instance.OnUpdateFrame(e, Keyboard);
                    ProcessarObjetos(e);
                }
            }

            ProcessarHUD();

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

            DesenharHUD();

            SwapBuffers();
        }

        private void DesenharHUD()
        {
            GL.MatrixMode(MatrixMode.Projection);
            GL.PushMatrix();
            GL.LoadIdentity();
            GL.Ortho(0.0, Width, 0.0, Height, 0.0, 4.0);

            GL.MatrixMode(MatrixMode.Modelview);
            GL.PushMatrix();
            GL.LoadIdentity();

            foreach (Objeto objeto in ObjetosHUD)
            {
                objeto.Desenhar();
            }

            GL.MatrixMode(MatrixMode.Projection);
            GL.PopMatrix();

            GL.MatrixMode(MatrixMode.Modelview);
            GL.PopMatrix();
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
                case Key.C:
                    if (e.Control)
                    {
                        CameraLivre = !CameraLivre;
                        if (CameraLivre)
                        {
                            PodeProcessarObjetos = false;
                        }
                    }

                    break;

                case Key.D:
                    if (e.Control)
                    {
                        Dalton = !Dalton;
                    }

                    break;

                case Key.P:
                    if (e.Control)
                    {
                        PodeProcessarObjetos = !PodeProcessarObjetos;
                    }

                    break;

                case Key.R:
                    _esfera.ObjetoCor.CorR++;
                    break;
                case Key.F:
                    _esfera.ObjetoCor.CorR--;
                    break;
                case Key.T:
                    _esfera.ObjetoCor.CorG++;
                    break;
                case Key.G:
                    _esfera.ObjetoCor.CorG--;
                    break;
                case Key.Y:
                    _esfera.ObjetoCor.CorB++;
                    break;
                case Key.H:
                    _esfera.ObjetoCor.CorB--;
                    break;
            }
        }

        private void ProcessarCameraTeclado(FrameEventArgs e)
        {
            if (Keyboard[Key.W])
            {
                Camera.Eye += Camera.At * CameraSpeed * (float) e.Time;
            }

            if (Keyboard[Key.S])
            {
                Camera.Eye -= Camera.At * CameraSpeed * (float) e.Time;
            }

            if (Keyboard[Key.A])
            {
                Camera.Eye -= Vector3.Normalize(Vector3.Cross(Camera.At, Camera.Up)) * CameraSpeed * (float) e.Time;
            }

            if (Keyboard[Key.D])
            {
                Camera.Eye += Vector3.Normalize(Vector3.Cross(Camera.At, Camera.Up)) * CameraSpeed * (float) e.Time;
            }

            if (Keyboard[Key.Space])
            {
                Camera.Eye += Camera.Up * CameraSpeed * (float) e.Time;
            }

            if (Keyboard[Key.LShift])
            {
                Camera.Eye -= Camera.Up * CameraSpeed * (float) e.Time;
            }

            if (Keyboard[Key.Plus] || Keyboard[Key.KeypadPlus])
            {
                CameraSpeed++;
            }

            if (Keyboard[Key.Minus] || Keyboard[Key.KeypadMinus])
            {
                CameraSpeed--;
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
            ProcessarColisaoObjetos(e);
            foreach (Objeto objeto in Objetos)
            {
                objeto.UpdateFrame(e);
            }
        }

        private void ProcessarColisaoObjetos(FrameEventArgs e)
        {
            for (int i = 0; i < Objetos.Count; i++)
            {
                Objeto objetoA = Objetos[i];
                for (int x = i + 1; x < Objetos.Count; x++)
                {
                    Objeto objetoB = Objetos[x];
                    ProcessarColisaoObjetos(e, objetoA, objetoB);
                }
            }
        }

        private void ProcessarColisaoObjetos(FrameEventArgs e, Objeto a, Objeto b)
        {
            if (a.Colisor != null && b.Colisor != null)
            {
                a.Colisor.ProcessarColisao(e, b);
            }

            IReadOnlyList<Objeto> filhosA = a.GetFilhos();
            foreach (Objeto filhoA in filhosA)
            {
                ProcessarColisaoObjetos(e, filhoA, b);

                IReadOnlyList<Objeto> filhosB = b.GetFilhos();
                foreach (Objeto filhoB in filhosB)
                {
                    ProcessarColisaoObjetos(e, filhoA, filhoB);
                }
            }
        }

        private void ProcessarHUD()
        {
            Time time1 = Jogo.Instance.Times[0];
            Time time2 = Jogo.Instance.Times[1];

            List<string> textos = new List<string>();
            textos.Add("Time 1:");
            textos.Add("  Pontos: " + time1.Pontos);
            textos.Add("  Bolas: " + time1.Bolas);
            textos.Add("  Jogadores: ");
            for (var i = 0; i < time1.Jogadores.Count; i++)
            {
                textos.Add("    Jogador " + (i + 1) + ": " + time1.Jogadores[i]);
            }

            textos.Add(" ");

            textos.Add("Time 2:");
            textos.Add("  Pontos: " + time2.Pontos);
            textos.Add("  Bolas: " + time2.Bolas);
            textos.Add("  Jogadores: ");
            for (var i = 0; i < time2.Jogadores.Count; i++)
            {
                textos.Add("    Jogador " + (i + 1) + ": " + time2.Jogadores[i]);
            }

            textos.Add(" ");
            textos.Add("Time atual: " + (Jogo.Instance.GetIdxTimeAtual() + 1));

            Esfera bochaAtual = Jogo.Instance.BolaAtual;
            if (bochaAtual != null && bochaAtual.Raio != BolaFactory.RaioBolin)
            {
                Esfera bolin = Jogo.Instance.GetBolin(Objetos);
                double distancia = Matematica.Distancia(bolin.BBox.obterCentro, bochaAtual.BBox.obterCentro);

                Vector3 v = bochaAtual.ForcaFisica.Velocidade;
                float va = Math.Abs(v.X) + Math.Abs(v.Y) + Math.Abs(v.Z);
                float km = va / 27.77778f;
                textos.Add(" ");
                textos.Add("Distância Bolin: " + Math.Round(distancia, 2) + "cm"
                           + "\nVelocidade: " + Math.Round(km, 2) + "km/h");
            }

            if (Atirador.Instance.IsPosicionando())
            {
                textos.Add(" ");
                textos.Add("Ângulo: " + Math.Round(Atirador.Instance.Angulo, 0) + "º");
                textos.Add("Força: " + Math.Round(Atirador.Instance.Forca * 100, 0) + "%");
            }

            if (TextoCentral != null)
            {
                textos.Add(TextoCentral);
            }

            Dictionary<string, Queue<Texto2D>> dic = new Dictionary<string, Queue<Texto2D>>();
            foreach (Texto2D texto2D in ObjetosHUD)
            {
                if (!textos.Contains(texto2D.Texto))
                {
                    texto2D.Dispose();
                }
                else
                {
                    if (!dic.ContainsKey(texto2D.Texto))
                    {
                        dic.Add(texto2D.Texto, new Queue<Texto2D>());
                    }

                    dic[texto2D.Texto].Enqueue(texto2D);
                }
            }

            ObjetosHUD.Clear();

            float y = Height;
            foreach (string texto in textos)
            {
                Texto2D texto2D;
                if (dic.ContainsKey(texto))
                {
                    Queue<Texto2D> queue = dic[texto];
                    texto2D = queue.Dequeue();
                    texto2D.AtribuirMatrizIdentidade();
                    if (queue.Count == 0)
                    {
                        dic.Remove(texto);
                    }
                }
                else
                {
                    texto2D = new Texto2D(texto);
                }

                y -= texto2D.Height;
                if (!texto.Equals(" "))
                {
                    texto2D.Translacao(0, y, 0);
                    ObjetosHUD.Add(texto2D);
                }
            }

            if (TextoCentral != null)
            {
                Texto2D textoCentral = ObjetosHUD.Find(t => t.Texto.Equals(TextoCentral));
                textoCentral.AtribuirMatrizIdentidade();
                textoCentral.Translacao(
                    (Width / 2) - (textoCentral.Width / 2),
                    (Height / 2) - (textoCentral.Height / 2),
                    0
                );
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

        public void ObjetosAdicionar(Objeto objeto)
        {
            if (!Objetos.Contains(objeto))
            {
                Objetos.Add(objeto);
            }
        }

        public void ObjetosRemover(Objeto objeto)
        {
            Objetos.Remove(objeto);
        }

        public IReadOnlyList<Objeto> GetObjetos()
        {
            return Objetos.AsReadOnly();
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