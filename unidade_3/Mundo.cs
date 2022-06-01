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
    private char PontoControleSelecionado = (char)100;

    
    
#if CG_Privado
    private Privado_SegReta obj_SegReta;
    private Privado_Circulo obj_Circulo;
#endif

    protected override void OnLoad(EventArgs e)
    {
      base.OnLoad(e);
      camera.xmin = -400;
      camera.xmax = 400;
      camera.ymin = -400; 
      camera.ymax = 400;

      Console.WriteLine(" --- Ajuda / Teclas: ");
      Console.WriteLine(" [  H     ] mostra teclas usadas. ");

      objetoId = Utilitario.charProximo(objetoId);
      
      var linhaVerticalGizmo = new Linha(objetoId, null,  new Ponto4D(), new Ponto4D(0, 200));
      linhaVerticalGizmo.ObjetoCor = new Cor(0, 150, 0);
      linhaVerticalGizmo.PrimitivaTamanho = 2;
      objetosLista.Add(linhaVerticalGizmo);
      
      var linhaHorizontalGizmo = new Linha(objetoId, null,  new Ponto4D(), new Ponto4D(200));
      linhaHorizontalGizmo.ObjetoCor = new Cor(255, 0, 0);
      linhaHorizontalGizmo.PrimitivaTamanho = 2;
      objetosLista.Add(linhaHorizontalGizmo);
      
      var linhaEsquerda = new SegReta((char)1000, null, new Ponto4D(-100, -100), new Ponto4D(-100, 100));
      linhaEsquerda.ObjetoCor = new Cor(0, 255, 255);
      linhaEsquerda.PrimitivaTamanho = 2;
      objetosLista.Add(linhaEsquerda);
      
      var linhaDireita = new SegReta((char)2000, null, new Ponto4D(100, 100), new Ponto4D(100, -100));
      linhaDireita.ObjetoCor = new Cor(0, 255, 255);
      linhaDireita.PrimitivaTamanho = 2;
      objetosLista.Add(linhaDireita);
      
      var linhaDeLigacao = new SegReta((char)5000, null, linhaEsquerda.PontoB, linhaDireita.PontoA);
      linhaDeLigacao.ObjetoCor = new Cor(0, 255, 255);
      linhaDeLigacao.PrimitivaTamanho = 2;
      objetosLista.Add(linhaDeLigacao);
      
      var pontoControleLinhaEsquerdaInferior = new Ponto((char)100, null, linhaEsquerda.PontoA);
      pontoControleLinhaEsquerdaInferior.ObjetoCor = new Cor(255, 0, 0);
      pontoControleLinhaEsquerdaInferior.PrimitivaTamanho = 8;
      objetosLista.Add(pontoControleLinhaEsquerdaInferior);
      
      var pontoControleLinhaEsquerdaSuperior = new Ponto((char)200, null, linhaEsquerda.PontoB);
      pontoControleLinhaEsquerdaSuperior.ObjetoCor = new Cor(0, 0, 0);
      pontoControleLinhaEsquerdaSuperior.PrimitivaTamanho = 8;
      objetosLista.Add(pontoControleLinhaEsquerdaSuperior);
      
      var pontoControleLinhaDireitaInferior = new Ponto((char)300, null, linhaDireita.PontoA);
      pontoControleLinhaDireitaInferior.ObjetoCor = new Cor(0, 0, 0);
      pontoControleLinhaDireitaInferior.PrimitivaTamanho = 8;
      objetosLista.Add(pontoControleLinhaDireitaInferior);
      
      var pontoControleLinhaDireitaSuperior = new Ponto((char)400, null, linhaDireita.PontoB);
      pontoControleLinhaDireitaSuperior.ObjetoCor = new Cor(0, 0, 0);
      pontoControleLinhaDireitaSuperior.PrimitivaTamanho = 8;
      objetosLista.Add(pontoControleLinhaDireitaSuperior);

      var spline = new Spline((char)8900, null, linhaEsquerda, linhaDeLigacao, linhaDireita);
      spline.ObjetoCor = new Cor(255, 255,0);
      spline.PrimitivaTamanho = 2;
      objetosLista.Add(spline);
      
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
        // camera.PanEsquerda();
        
        var segReta = ObterSegRetaCorrespontendeAoPontoControleSelecionado();
        var pontoDeControleSelecionado = (Ponto)objetosLista.FirstOrDefault(w => w.Rotulo == PontoControleSelecionado);
        pontoDeControleSelecionado?.MoverParaEsquerda(1);
        
        SegReta segRetaLigacao;
        switch (PontoControleSelecionado)
        { 
          case (char)100:
            segReta.MoverPontoAParaEsquerda(1);
            break;
          case (char)200:
            segReta.MoverPontoBParaEsquerda(1);
            segRetaLigacao = (SegReta)objetosLista.FirstOrDefault(w => w.Rotulo == (char)5000);
            segRetaLigacao?.MoverPontoAParaEsquerda(1);
            break;
          case (char)300:
            segReta.MoverPontoAParaEsquerda(1);
            segRetaLigacao = (SegReta)objetosLista.FirstOrDefault(w => w.Rotulo == (char)5000);
            segRetaLigacao?.MoverPontoBParaEsquerda(1);
            break;
          case (char)400:
            segReta.MoverPontoBParaEsquerda(1);
            break;
          default: throw new NullReferenceException();
        }
        
      }
      else if (e.Key == Key.D)
      {
        // camera.PanDireita();
        
        var segReta = ObterSegRetaCorrespontendeAoPontoControleSelecionado();
        var pontoDeControleSelecionado = (Ponto)objetosLista.FirstOrDefault(w => w.Rotulo == PontoControleSelecionado);
        pontoDeControleSelecionado?.MoverParaDireita(1);
        
        SegReta segRetaLigacao;
        switch (PontoControleSelecionado)
        { 
          case (char)100:
            segReta.MoverPontoAParaDireita(1);
            break;
          case (char)200:
            segReta.MoverPontoBParaDireita(1);
            segRetaLigacao = (SegReta)objetosLista.FirstOrDefault(w => w.Rotulo == (char)5000);
            segRetaLigacao?.MoverPontoAParaDireita(1);
            break;
          case (char)300:
            segReta.MoverPontoAParaDireita(1);
            segRetaLigacao = (SegReta)objetosLista.FirstOrDefault(w => w.Rotulo == (char)5000);
            segRetaLigacao?.MoverPontoBParaDireita(1);
            break;
          case (char)400:
            segReta.MoverPontoBParaDireita(1);
            break;
          default: throw new NullReferenceException();
        }
        
      }else if (e.Key == Key.C)
      {
        // camera.PanCima();

        var segReta = ObterSegRetaCorrespontendeAoPontoControleSelecionado();
        var pontoDeControleSelecionado = (Ponto)objetosLista.FirstOrDefault(w => w.Rotulo == PontoControleSelecionado);
        pontoDeControleSelecionado?.MoverParaCima(1);
        
        SegReta segRetaLigacao;
        switch (PontoControleSelecionado)
        { 
          case (char)100:
            segReta.MoverPontoAParaCima(1);
            break;
          case (char)200:
            segReta.MoverPontoBParaCima(1);
            segRetaLigacao = (SegReta)objetosLista.FirstOrDefault(w => w.Rotulo == (char)5000);
            segRetaLigacao?.MoverPontoAParaCima(1);
            break;
          case (char)300:
            segReta.MoverPontoAParaCima(1);
            segRetaLigacao = (SegReta)objetosLista.FirstOrDefault(w => w.Rotulo == (char)5000);
            segRetaLigacao?.MoverPontoBParaCima(1);
            break;
          case (char)400:
            segReta.MoverPontoBParaCima(1);
            break;
          default: throw new NullReferenceException();
        }
      }
      else if(e.Key == Key.B)
      {
        // camera.PanBaixo();
        
        var segReta = ObterSegRetaCorrespontendeAoPontoControleSelecionado();
        var pontoDeControleSelecionado = (Ponto)objetosLista.FirstOrDefault(w => w.Rotulo == PontoControleSelecionado);
        pontoDeControleSelecionado?.MoverParaBaixo(1);
        
        SegReta segRetaLigacao;
        switch (PontoControleSelecionado)
        { 
          case (char)100:
            segReta.MoverPontoAParaBaixo(1);
            break;
          case (char)200:
            segReta.MoverPontoBParaBaixo(1);
            segRetaLigacao = (SegReta)objetosLista.FirstOrDefault(w => w.Rotulo == (char)5000);
            segRetaLigacao?.MoverPontoAParaBaixo(1);
            break;
          case (char)300:
            segReta.MoverPontoAParaBaixo(1);
            segRetaLigacao = (SegReta)objetosLista.FirstOrDefault(w => w.Rotulo == (char)5000);
            segRetaLigacao?.MoverPontoBParaBaixo(1);
            break;
          case (char)400:
            segReta.MoverPontoBParaBaixo(1);
            break;
          default: throw new NullReferenceException();
        }
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
      else if (e.Key == Key.R)
      {
        Resetar();
      }
      else if (e.Key == Key.O)
      {
        bBoxDesenhar = !bBoxDesenhar;
      }
      else if (e.Key == Key.V)
      {
        mouseMoverPto = !mouseMoverPto; 
      }
      else if (e.Key == Key.Number1)
      {
        SelecionarPontoControle((char)100);
      }
      else if (e.Key == Key.Number2)
      {
        SelecionarPontoControle((char)200);
      }
      else if (e.Key == Key.Number3)
      {
        SelecionarPontoControle((char)300);
      }
      else if (e.Key == Key.Number4)
      {
        SelecionarPontoControle((char)400);
      }
      else if (e.Key == Key.Plus)
      {
        var spline = (Spline)objetosLista.FirstOrDefault(w => w.Rotulo == (char)8900);
        spline.AumentarQuantidadePontos();
      }
      else if (e.Key == Key.Minus)
      {
        var spline = (Spline)objetosLista.FirstOrDefault(w => w.Rotulo == (char)8900);
        spline.DiminuirQuantidadePontos();
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

    private SegReta ObterSegRetaCorrespontendeAoPontoControleSelecionado()
    {
      switch (PontoControleSelecionado)
      {
        case (char)100:
        case (char)200:
          return (SegReta)objetosLista.FirstOrDefault(w => w.Rotulo == (char)1000);
        case (char)300:
        case (char)400:
          return (SegReta)objetosLista.FirstOrDefault(w => w.Rotulo == (char)2000);
        default: throw new NullReferenceException();
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

    private void SelecionarPontoControle(char rotulo)
    {
      var pontoControle = (Ponto)objetosLista.FirstOrDefault(w => w.Rotulo == rotulo);
      pontoControle.ObjetoCor = new Cor(255, 0, 0);
      PontoControleSelecionado = pontoControle.Rotulo;

      var pontosDeControleParaDesabilitarCorVermelha = new List<char>{ (char)100, (char)200, (char)300, (char)400 };
      pontosDeControleParaDesabilitarCorVermelha.Remove(rotulo);
      
      pontosDeControleParaDesabilitarCorVermelha.ForEach(DesmarcarSelecaoPontoControle);
    }
    private void DesmarcarSelecaoPontoControle(char rotulo)
    {
      var pontoControle = (Ponto)objetosLista.FirstOrDefault(w => w.Rotulo == rotulo);
      pontoControle.ObjetoCor = new Cor(0, 0, 0);
    }
    
    private void Resetar()
    {
      
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
