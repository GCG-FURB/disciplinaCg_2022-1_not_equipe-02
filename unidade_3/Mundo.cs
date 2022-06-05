/**
  Autor: Dalton Solano dos Reis
**/

// #define CG_Gizmo
// #define CG_Privado

using System;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System.Collections.Generic;
using OpenTK.Input;
using CG_Biblioteca;

namespace gcgcg
{
  class Mundo : GameWindow
  {
    private static Mundo instanciaMundo = null;
    private static readonly char RotuloSenhorPalito = Utilitario.charProximo((char)999);

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
    private ObjetoGeometria objetoSelecionado = null;
    private char objetoId = '@';
    private bool bBoxDesenhar = false;
    int mouseX, mouseY; //TODO: achar método MouseDown para não ter variável Global
    private bool mouseMoverPto = false;
    
    private Poligono poligonoSendoDesenhado;
    private bool estaSendoDesenhadoPoligono = false;

#if CG_Privado
    private Privado_SegReta obj_SegReta;
    private Privado_Circulo obj_Circulo;
#endif

    protected override void OnLoad(EventArgs e)
    {
      base.OnLoad(e);
      camera.xmin = 0;
      camera.xmax = 600;
      camera.ymin = 0; 
      camera.ymax = 600;

      Console.WriteLine(" --- Ajuda / Teclas: ");
      Console.WriteLine(" [  H     ] mostra teclas usadas. ");

      objetoId = Utilitario.charProximo(objetoId);
      poligonoSendoDesenhado = new Poligono(objetoId, null);
      objetosLista.Add(poligonoSendoDesenhado);
      
      
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
      this.SwapBuffers();
    }

    protected override void OnKeyDown(KeyboardKeyEventArgs e)
    {
      if (e.Key == Key.H)
        Utilitario.AjudaTeclado();
      else if (e.Key == Key.Escape)
        Exit();
      else if (e.Key == Key.I)
      {
        camera.ZoomIn(); 
      }
      else if (e.Key == Key.O)
      {
        camera.ZoomOut();
      }
      else if (e.Key == Key.O)
      {
        bBoxDesenhar = !bBoxDesenhar;
      }
      else if (e.Key == Key.V)
      {
        mouseMoverPto = !mouseMoverPto; 
      }
      else if (e.Key == Key.Space)
      {
        estaSendoDesenhadoPoligono = true;
        
        var mouseXMomento = this.mouseX;
        var mouseYMomento = this.mouseY;
        
        poligonoSendoDesenhado.PontosAdicionar(new Ponto4D(mouseXMomento, mouseYMomento));
        poligonoSendoDesenhado.PontosAdicionar(new Ponto4D(mouseXMomento, mouseYMomento));
      }
      else if (e.Key == Key.Enter)
      {
        NovoPoligono();
      }
      else
      {
        Console.WriteLine(" __ Tecla não implementada.");
      }
    }

    //TODO: não está considerando o NDC
    protected override void OnMouseMove(MouseMoveEventArgs e)
    {
      mouseX = e.Position.X;
      mouseY = 600 - e.Position.Y; // Inverti eixo Y
      // if (mouseMoverPto && (objetoSelecionado != null))
      // {
      //   objetoSelecionado.PontosUltimo().X = mouseX;
      //   objetoSelecionado.PontosUltimo().Y = mouseY;
      // }

      if (estaSendoDesenhadoPoligono)
      {
        poligonoSendoDesenhado.ModificarCoordenadaUltimoPonto(mouseX, mouseY);
      }
    }

    public void NovoPoligono()
    {
      estaSendoDesenhadoPoligono = false;
      poligonoSendoDesenhado.PontosRemoverUltimo();
      poligonoSendoDesenhado.PontosRemoverUltimo();
      
      poligonoSendoDesenhado = new Poligono(Utilitario.charProximo(poligonoSendoDesenhado.Rotulo), null);
      objetosLista.Add(poligonoSendoDesenhado);
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
      window.Title = "CG_N3";
      window.Run(1.0 / 60.0);
    }
  }
}
