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
    private static readonly char RotuloSenhorPalito = Utilitario.charProximo((char)999);
    
    private Mundo(int width, int height) : base(width, height) { }

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
    int mouseX, mouseY;   //TODO: achar método MouseDown para não ter variável Global
    private bool mouseMoverPto = false;
    private Retangulo obj_Retangulo;

    
    
#if CG_Privado
    private Privado_SegReta obj_SegReta;
    private Privado_Circulo obj_Circulo;
#endif

    protected override void OnLoad(EventArgs e)
    {
      base.OnLoad(e);
      camera.xmin = -600;
      camera.xmax = 600;
      camera.ymin = -600; 
      camera.ymax = 600;

      Console.WriteLine(" --- Ajuda / Teclas: ");
      Console.WriteLine(" [  H     ] mostra teclas usadas. ");

      objetoId = Utilitario.charProximo(objetoId);

      var linhaVertical = new SegReta(objetoId, null,  new Ponto4D(0), new Ponto4D(0,200));
      linhaVertical.ObjetoCor = new Cor(0, 150, 0);
      linhaVertical.PrimitivaTamanho = 5;
      objetosLista.Add(linhaVertical);
      
      var linhaHorizontal = new SegReta(Utilitario.charProximo(objetoId), null,  new Ponto4D(0), new Ponto4D(200));
      linhaHorizontal.ObjetoCor = new Cor(255, 0, 0);
      linhaHorizontal.PrimitivaTamanho = 5;
      objetosLista.Add(linhaHorizontal);

      
      var senhorPalito = new SegReta(RotuloSenhorPalito, null, new Ponto4D(), (anguloPontoB: 45, raioPontoB: 100));
      senhorPalito.PrimitivaTamanho = 5;
      senhorPalito.ObjetoCor = new Cor(0, 0, 0);
      objetosLista.Add(senhorPalito);
      
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

    protected override void OnKeyDown(OpenTK.Input.KeyboardKeyEventArgs e)
    {
      if (e.Key == Key.H)
        Utilitario.AjudaTeclado();
      else if (e.Key == Key.Escape)
        Exit();
      else if (e.Key == Key.E)
      {
        camera.PanEsquerda();
      }
      else if (e.Key == Key.D)
      {
        camera.PanDireita();
      }else if (e.Key == Key.C)
      {
        camera.PanCima();
      }
      else if(e.Key == Key.B)
      {
        camera.PanBaixo();
      }
      else if (e.Key == Key.I)
      {
        camera.ZoomIn(); 
      }
      else if (e.Key == Key.O)
      {
        camera.ZoomOut();
      }
      else if (e.Key == Key.Space)
      {
        // Utilitario.ModificarPrimitivaEscolhida();
        // var exercicio = (Exercicio04)objetosLista[2];
        // exercicio.PrimitivaTipo = Utilitario.ObterPrimitivaAtual();
      }
      else if (e.Key == Key.Q)
      {
        var senhorPalito = ObterPalito();
        senhorPalito.MoverParaEsquerda(unidadesParaMover: 1);
      }
      else if (e.Key == Key.W)
      {
        var senhorPalito = ObterPalito();
        senhorPalito.MoverParaDireita(unidadesParaMover: 1);
      }
      else if (e.Key == Key.A)
      {
        var senhorPalito = ObterPalito();
        senhorPalito.DiminuirRaioPontoB();
      }
      else if (e.Key == Key.S)
      {
        var senhorPalito = ObterPalito();
        senhorPalito.AumentarRaioPontoB();
      }
      else if (e.Key == Key.Z)
      {
        var senhorPalito = ObterPalito();
        senhorPalito.DiminuirAnguloPontoB();
      }
      else if (e.Key == Key.X)
      {
        var senhorPalito = ObterPalito();
        senhorPalito.AumentarAnguloPontoB();
      }
      else if (e.Key == Key.O)
      {
        bBoxDesenhar = !bBoxDesenhar;
      }
      else if (e.Key == Key.V)
      {
        mouseMoverPto = !mouseMoverPto; 
      }
      else
      {
        Console.WriteLine(" __ Tecla não implementada.");
      }
    }

    //TODO: não está considerando o NDC
    protected override void OnMouseMove(MouseMoveEventArgs e)
    {
      mouseX = e.Position.X; mouseY = 600 - e.Position.Y; // Inverti eixo Y
      if (mouseMoverPto && (objetoSelecionado != null))
      {
        objetoSelecionado.PontosUltimo().X = mouseX;
        objetoSelecionado.PontosUltimo().Y = mouseY;
      }
    }

    private SegReta ObterPalito()
    {
      foreach (var obj in objetosLista)
      {
        var segReta = (SegReta)obj;
        if (segReta != null && segReta.Rotulo == RotuloSenhorPalito)
        {
          return segReta;
        }
      }

      throw new ArgumentNullException("Palito não encontrado");
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
