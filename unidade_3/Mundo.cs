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
    private bool ehPrimeiroPontoNoPoligono = true;
    private bool estaSendoAlteradoPontoPoligonoPelaTeclaV = false;

    private (double menorDistancia, Poligono poligonoMenorDistancia, Ponto4D coordenadaMenorDistancia, int indexPontoMenorDistancia) ResultadoCalculoDistanciaParaMovimentarVerticeMaisProximo;

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
   
    protected override void OnResize(EventArgs e)
    {
      base.OnResize(e);
      GL.Viewport(0, 0, 600, 600);
    }

    protected override void OnMouseMove(MouseMoveEventArgs e)
    {
      mouseX = e.X;
      mouseY = 600 - e.Y; // Inverti eixo Y

      var coordenadaMouseAtual = new Ponto4D(mouseX, mouseY);
      
      if (estaSendoDesenhadoPoligono)
      {
        poligonoSendoDesenhado.ModificarCoordenadaUltimoPonto(coordenadaMouseAtual.X, coordenadaMouseAtual.Y);
        return;
      }

      if (estaSendoAlteradoPontoPoligonoPelaTeclaV)
      {
        ResultadoCalculoDistanciaParaMovimentarVerticeMaisProximo.poligonoMenorDistancia.PontosAlterar(coordenadaMouseAtual, ResultadoCalculoDistanciaParaMovimentarVerticeMaisProximo.indexPontoMenorDistancia);
      }
    }

     protected override void OnKeyDown(KeyboardKeyEventArgs e)
    {
      if (e.Key == Key.H)
        Utilitario.AjudaTeclado();
      else if (e.Key == Key.Escape)
        Exit();
      else if (e.Key == Key.O)
      {
        bBoxDesenhar = !bBoxDesenhar;
      }
      else if (e.Key == Key.Space)
      {
        estaSendoDesenhadoPoligono = true;
        
        var mouseXMomento = this.mouseX;
        var mouseYMomento = this.mouseY;
        
        poligonoSendoDesenhado.PontosAdicionar(new Ponto4D(mouseXMomento, mouseYMomento));
        if (ehPrimeiroPontoNoPoligono)
        {
          poligonoSendoDesenhado.PontosAdicionar(new Ponto4D(mouseXMomento, mouseYMomento));
          ehPrimeiroPontoNoPoligono = false;
        }
      }
      else if (e.Key == Key.Enter)
      {
        IniciarDesenhoNovoPoligono();
      }
      else if (e.Key == Key.A)
      {
        SelecionarPoligonoAPartirPosicaoMouse(mouseX, mouseY);
      }
      else if (e.Key == Key.S)
      {
        var poligonoSelecionado = (Poligono)objetoSelecionado;
        poligonoSelecionado?.AlternarPrimitivaTipoEntreLineStripELineLoop();
      }
      else if (e.Key == Key.R)
      {
        if (objetoSelecionado != null)
        {
          objetoSelecionado.ObjetoCor = new Cor(255, 0, 0);
        }
      }
      else if (e.Key == Key.G)
      {
        if (objetoSelecionado != null)
        {
          objetoSelecionado.ObjetoCor = new Cor(0, 250, 0);
        }
      }
      else if (e.Key == Key.B)
      {
        if (objetoSelecionado != null)
        {
          objetoSelecionado.ObjetoCor = new Cor(0, 0 , 255);
        }
      }
      else if (e.Key == Key.Left)
      { 
        objetoSelecionado?.AtribuirTranslacao(-10, 0, 0);
      }
      else if (e.Key == Key.Right)
      {
        objetoSelecionado?.AtribuirTranslacao(10, 0, 0);
      }
      else if (e.Key == Key.Up)
      {
        objetoSelecionado?.AtribuirTranslacao(0, 10, 0);
      }
      else if (e.Key == Key.Down)
      {
        objetoSelecionado?.AtribuirTranslacao(0, -10, 0);
      }
      else if (e.Key == Key.X)
      {
        objetoSelecionado?.AtribuirRotacao(EixoRotacao.X, 358);
      }
      else if (e.Key == Key.PageUp)
      {
        
        objetoSelecionado?.AtribuirEscala(2,2,2);
      }
      else if (e.Key == Key.PageDown)
      {
        objetoSelecionado?.AtribuirEscala(-2,0,0);
      }
      else if (e.Key == Key.V)
      {
        ResultadoCalculoDistanciaParaMovimentarVerticeMaisProximo = ObterVerticeMaisProximoMouse(mouseX, mouseY);
        estaSendoAlteradoPontoPoligonoPelaTeclaV = true;
      }
      else
      {
        Console.WriteLine(" __ Tecla não implementada.");
      }
    }
    
    private void IniciarDesenhoNovoPoligono()
    {
      if (estaSendoAlteradoPontoPoligonoPelaTeclaV)
      {
        estaSendoAlteradoPontoPoligonoPelaTeclaV = false;
        return;
      }
      
      if (!estaSendoDesenhadoPoligono)
      {
        return;
      }
      
      estaSendoDesenhadoPoligono = false;
      poligonoSendoDesenhado.PontosRemoverUltimo();
      objetoSelecionado = poligonoSendoDesenhado;
      
      ehPrimeiroPontoNoPoligono = true;

      objetoId = Utilitario.charProximo(objetoId);
      poligonoSendoDesenhado = new Poligono(objetoId, null);

      AdicionarPoligonoAoMundoOuComoFilhoDoObjetoSelecionado(poligonoSendoDesenhado);
    }

    private void SelecionarPoligonoAPartirPosicaoMouse(double posicaoXMouse, double posicaoYMouse)
    {
      var pontoClique = new Ponto4D(posicaoXMouse, posicaoYMouse);
      var poligonos = objetosLista.Where(w => typeof(Poligono) == w.GetType());

      foreach (Poligono poligono in poligonos)
      {
        var retornoValidacaoEstaDentro = poligono.VerificarSeCoordenadaEstaDentro(pontoClique);
        if (retornoValidacaoEstaDentro.EstaDentro)
        {
          objetoSelecionado = retornoValidacaoEstaDentro.poligonoSelecionado;
          break;
        }
      }
    }
    
    private void AdicionarPoligonoAoMundoOuComoFilhoDoObjetoSelecionado(Poligono poligono)
    {
      if (objetoSelecionado == null)
      {
        objetosLista.Add(poligono);
        return;
      }
      
      objetoSelecionado.FilhoAdicionar(poligono);
    }
    
    private (double menorDistancia, Poligono poligonoMenorDistancia, Ponto4D coordenadaMenorDistancia, int indexPontoMenorDistancia) ObterVerticeMaisProximoMouse(int mouseX, int mouseY)
    {
      var pontoCoordenadaMouse = new Ponto4D(mouseX, mouseY);
      (double menorDistancia, Poligono poligonoMenorDistancia, Ponto4D coordenadaMenorDistancia, int indexPontoMenorDistancia) ResultadoCalculoDistancia = (double.MaxValue, null, new Ponto4D(), 0);
      
      var poligonos = objetosLista.Where(w => typeof(Poligono) == w.GetType());
      
      foreach (Poligono poligono in poligonos)
      {
        var pontosPoligono = poligono.ObterPontos();
        for (var i = 0; i < pontosPoligono.Count; i++)
        {
          var distancia = Matematica.Distancia(pontoCoordenadaMouse, pontosPoligono[i]);
          if (distancia <= ResultadoCalculoDistancia.menorDistancia)
          {
            ResultadoCalculoDistancia.menorDistancia = distancia;
            ResultadoCalculoDistancia.poligonoMenorDistancia = poligono;
            ResultadoCalculoDistancia.coordenadaMenorDistancia = pontosPoligono[i];
            ResultadoCalculoDistancia.indexPontoMenorDistancia = i;
          }
        }

        var objetosFilhos = poligono.ObterObjetosFilhos();
        foreach (Poligono poligonoFilho in objetosFilhos)
        {
          var pontosDoPoligonoFilho = poligonoFilho.ObterPontos();
          for (var i = 0; i < pontosDoPoligonoFilho.Count; i++)
          {
            var distancia = Matematica.Distancia(pontoCoordenadaMouse, pontosDoPoligonoFilho[i]);
            if (distancia <= ResultadoCalculoDistancia.menorDistancia)
            {
              ResultadoCalculoDistancia.menorDistancia = distancia;
              ResultadoCalculoDistancia.poligonoMenorDistancia = poligonoFilho;
              ResultadoCalculoDistancia.coordenadaMenorDistancia = pontosPoligono[i];
              ResultadoCalculoDistancia.indexPontoMenorDistancia = i;
            }
          }
        }
       
      }
      return ResultadoCalculoDistancia;
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
      ToolkitOptions.Default.EnableHighResolution = false;

      Mundo window = Mundo.GetInstance(600, 600);
      window.Title = "CG_N3";
      window.Run(1.0 / 60.0);
    }
  }
}
