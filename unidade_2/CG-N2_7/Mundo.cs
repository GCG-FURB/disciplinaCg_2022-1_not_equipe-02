/**
  Autor: Dalton Solano dos Reis
**/

// #define CG_Gizmo
// #define CG_Privado

using System;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System.Collections.Generic;
using System.Linq;
using OpenTK.Input;
using CG_Biblioteca;

namespace gcgcg
{
    class Mundo : GameWindow
    {
        private static Mundo instanciaMundo = null;

        private Mundo(int width, int height) : base(width, height)
        {
        }

        public static Mundo GetInstance(int width, int height)
        {
            if (instanciaMundo == null)
                instanciaMundo = new Mundo(width, height);
            return instanciaMundo;
        }

        private CameraOrtho camera = new CameraOrtho();
        protected List<Objeto> objetosLista = new List<Objeto>();
        private Objeto objetoSelecionado = null;
        private char objetoId = '@';
        private bool bBoxDesenhar = true;

        private Circulo circuloFora;
        private Circulo circuloInterno;
        private Ponto pontoInterno;

        private int? mouseX, mouseY;
        private bool mousePressed;

#if CG_Privado
    private Privado_SegReta obj_SegReta;
    private Privado_Circulo obj_Circulo;
#endif

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            camera.xmin = -50;
            camera.xmax = 750;
            camera.ymin = -50;
            camera.ymax = 750;

            Console.WriteLine(" --- Ajuda / Teclas: ");
            Console.WriteLine(" [  H     ] mostra teclas usadas. ");

            objetoId = Utilitario.charProximo(objetoId);

            var linhaVerticalGizmo = new Linha(objetoId, null, new Ponto4D(), new Ponto4D(0, 200));
            linhaVerticalGizmo.ObjetoCor = new Cor(0, 150, 0);
            linhaVerticalGizmo.PrimitivaTamanho = 2;
            objetosLista.Add(linhaVerticalGizmo);

            objetoId = Utilitario.charProximo(objetoId);

            var linhaHorizontalGizmo = new Linha(objetoId, null, new Ponto4D(), new Ponto4D(200));
            linhaHorizontalGizmo.ObjetoCor = new Cor(255, 0, 0);
            linhaHorizontalGizmo.PrimitivaTamanho = 2;
            objetosLista.Add(linhaHorizontalGizmo);

            objetoId = Utilitario.charProximo(objetoId);

            circuloFora = new Circulo(objetoId, null, 360, 200, new Ponto4D(350, 350));
            circuloFora.ObjetoCor = new Cor(0, 0, 0);
            objetosLista.Add(circuloFora);

            objetoId = Utilitario.charProximo(objetoId);

            circuloInterno = new Circulo(objetoId, null, 180, 75, new Ponto4D(350, 350));
            circuloInterno.ObjetoCor = new Cor(0, 0, 0);
            objetosLista.Add(circuloInterno);

            objetoId = Utilitario.charProximo(objetoId);

            pontoInterno = new Ponto(objetoId, null, new Ponto4D(350, 350));
            circuloInterno.ObjetoCor = new Cor(0, 0, 0);
            objetosLista.Add(pontoInterno);

#if CG_Privado
      objetoId = Utilitario.charProximo(objetoId);
      obj_SegReta = new Privado_SegReta(objetoId, null, new Ponto4D(50, 150), new Ponto4D(150, 250));
      obj_SegReta.ObjetoCor.CorR = 255; obj_SegReta.ObjetoCor.CorG = 255; obj_SegReta.ObjetoCor.CorB = 0;
      objetosLista.Add(obj_SegReta);
      objetoSelecionado = obj_SegReta;

      objetoId = Utilitario.charProximo(objetoId);
      obj_Circulo = new Privado_Circulo(objetoId, null, new Ponto4D(100, 300), 50);
      obj_Circulo.ObjetoCor.CorR = 0; obj_Circulo.ObjetoCor.CorG = 255; obj_Circulo.ObjetoCor.CorB = 255;
      objetosLista.Add(obj_Circulo);
      objetoSelecionado = obj_Circulo;
#endif
            GL.ClearColor(0.5f, 0.5f, 0.5f, 1.0f);
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();
            GL.Ortho(camera.xmin, camera.xmax, camera.ymin, camera.ymax, camera.zmin, camera.zmax);
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);
            GL.Clear(ClearBufferMask.ColorBufferBit);
            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadIdentity();
#if CG_Gizmo
      Sru3D();
#endif
            for (var i = 0; i < objetosLista.Count; i++)
                objetosLista[i].Desenhar();
            if (bBoxDesenhar && (objetoSelecionado != null))
                objetoSelecionado.BBox.Desenhar();

            circuloFora.BBox.Desenhar();

            this.SwapBuffers();
        }

        protected override void OnKeyDown(OpenTK.Input.KeyboardKeyEventArgs e)
        {
            Console.WriteLine(" __ Tecla não implementada.");
        }

        //TODO: não está considerando o NDC
        protected override void OnMouseMove(MouseMoveEventArgs e)
        {
            mousePressed = e.Mouse.LeftButton == ButtonState.Pressed;
            if (mousePressed)
            {
                if (mouseX != null)
                {
                    var diffX = e.Position.X - mouseX ?? 0;
                    var diffY = e.Position.Y - mouseY ?? 0;
                    if (diffX != 0 || diffY != 0)
                    {
                        OnMouseDrag(e, diffX, diffY * -1);
                    }
                }

                mouseX = e.Position.X;
                mouseY = e.Position.Y;
            }
            else
            {
                mouseX = null;
                mouseY = null;
                Resetar();
            }
        }

        private void OnMouseDrag(MouseMoveEventArgs e, int x, int y)
        {
            if (isPossibleToDrag(x, y))
            {
                circuloInterno.Centro.X += x;
                circuloInterno.Centro.Y += y;

                pontoInterno.Ponto4D.X = circuloInterno.Centro.X;
                pontoInterno.Ponto4D.Y = circuloInterno.Centro.Y;
            }
        }

        private bool isPossibleToDrag(int x, int y)
        {
            var bbox = circuloFora.BBox;
            var ponto = pontoInterno.Ponto4D;
            if (ponto.X + x < bbox.obterMaiorX && ponto.X + x > bbox.obterMenorX
                && ponto.Y + y < bbox.obterMaiorY && ponto.Y + y > bbox.obterMenorY)
            {
                return true;
            }

            var pontoCentro = circuloFora.Centro;
            var distancia = Math.Sqrt(Math.Pow(pontoCentro.X - ponto.X - x, 2) + Math.Pow(pontoCentro.Y - ponto.Y - y, 2));
            if (distancia < 200) // menor que o raio
            {
                return true;
            }

            return false;
        }

        private void Resetar()
        {
            circuloInterno.Centro.X = 350;
            circuloInterno.Centro.Y = 350;
            pontoInterno.Ponto4D.X = circuloInterno.Centro.X;
            pontoInterno.Ponto4D.Y = circuloInterno.Centro.Y;
        }

#if CG_Gizmo
    private void Sru3D()
    {
      GL.LineWidth(1);
      GL.Begin(PrimitiveType.Lines);
      // GL.Color3(1.0f,0.0f,0.0f);
      GL.Color3(Convert.ToByte(255), Convert.ToByte(0), Convert.ToByte(0));
      GL.Vertex3(0, 0, 0); GL.Vertex3(200, 0, 0);
      // GL.Color3(0.0f,1.0f,0.0f);
      GL.Color3(Convert.ToByte(0), Convert.ToByte(255), Convert.ToByte(0));
      GL.Vertex3(0, 0, 0); GL.Vertex3(0, 200, 0);
      // GL.Color3(0.0f,0.0f,1.0f);
      GL.Color3(Convert.ToByte(0), Convert.ToByte(0), Convert.ToByte(255));
      GL.Vertex3(0, 0, 0); GL.Vertex3(0, 0, 200);
      GL.End();
    }
#endif
    }

    class Program
    {
        static void Main(string[] args)
        {
            Mundo window = Mundo.GetInstance(600, 600);
            window.Title = "CG_N2";
            window.Run(1.0 / 60.0);
        }
    }
}