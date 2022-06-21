/**
  Autor: Dalton Solano dos Reis
**/

using System;
using OpenTK.Graphics.OpenGL;
using System.Collections.Generic;
using CG_Biblioteca;
using OpenTK;

namespace gcgcg
{
  public abstract class Objeto
  {
    protected char rotulo;
    public char Rotulo { get; private set; }
    private Cor objetoCor = new Cor(255, 255, 255, 255);
    public Cor ObjetoCor { get => objetoCor; set => objetoCor = value; }
    private PrimitiveType primitivaTipo = PrimitiveType.LineLoop;
    public PrimitiveType PrimitivaTipo { get => primitivaTipo; set => primitivaTipo = value; }
    private float primitivaTamanho = 1;
    public float PrimitivaTamanho { get => primitivaTamanho; set => primitivaTamanho = value; }
    private BBox bBox = new BBox();
    public BBox BBox { get => bBox; set => bBox = value; }
    private List<Objeto> objetosLista = new List<Objeto>();
    
    private Transformacao4D MatrizTransformacao =  new Transformacao4D();
    private Transformacao4D MatrizTransformacaoTemporaria = new Transformacao4D();
    private static Transformacao4D matrizTmpTranslacao = new Transformacao4D();
    private static Transformacao4D matrizTmpTranslacaoInversa = new Transformacao4D();
    private static Transformacao4D matrizTmpRotacao = new Transformacao4D();
    private static Transformacao4D matrizGlobal = new Transformacao4D();
    
    public Objeto(char rotulo, Objeto paiRef)
    {
      this.rotulo = rotulo;
      Rotulo = rotulo;
    }

    public void Desenhar()
    {
      GL.PushMatrix();                                    
      GL.MultMatrix(MatrizTransformacao.ObterDados());
      GL.Color3(objetoCor.CorR, objetoCor.CorG, objetoCor.CorB);
      GL.LineWidth(primitivaTamanho);
      GL.PointSize(primitivaTamanho);
      DesenharGeometria();
      for (var i = 0; i < objetosLista.Count; i++)
      {
        objetosLista[i].Desenhar();
      }
      GL.PopMatrix();  
    }
    protected abstract void DesenharGeometria();
    public void FilhoAdicionar(Objeto filho)
    {
      this.objetosLista.Add(filho);
    }
    public void FilhoRemover(Objeto filho)
    {
      this.objetosLista.Remove(filho);
    }

    public void FilhoRemoverRecursivo(char rotuloFilhoRemover)
    {
      foreach (var objetoFilho in objetosLista)
      {
        if (objetoFilho.Rotulo == rotuloFilhoRemover)
        {
          objetosLista.Remove(objetoFilho);
          return;
        }
        objetoFilho.FilhoRemoverRecursivo(rotuloFilhoRemover);
      }
    }

    public void AtribuirTranslacao(double tx, double ty, double tz)
    {
      MatrizTransformacaoTemporaria.AtribuirTranslacao(tx, ty, tz);
      MatrizTransformacao = MatrizTransformacao.MultiplicarMatriz(MatrizTransformacaoTemporaria);
      MatrizTransformacaoTemporaria.AtribuirIdentidade();
    }

    public void AtribuirRotacao(EixoRotacao eixoRotacao, double angulo)
    {
      var matrizTemporariaRotacionada = AplicarRotacaoMatrizTemporaria(eixoRotacao, angulo);
      MatrizTransformacao = MatrizTransformacao.MultiplicarMatriz(matrizTemporariaRotacionada);
      matrizTemporariaRotacionada.AtribuirIdentidade();
    }

    public void AtribuirEscala(double sX, double sY,  double sZ)
    {
      MatrizTransformacaoTemporaria.AtribuirEscala(sX, sY, sZ);
      MatrizTransformacao = MatrizTransformacao.MultiplicarMatriz(MatrizTransformacaoTemporaria);
      MatrizTransformacaoTemporaria.AtribuirIdentidade();
    }

    public IReadOnlyCollection<Objeto> ObterObjetosFilhos() => objetosLista.AsReadOnly();

    public void AtribuirMatrizIdentidade() => MatrizTransformacao.AtribuirIdentidade();
    
    public void ImprimirMatrizTransformacao() => Console.WriteLine(MatrizTransformacao);
    
    public void RotacaoZBBox(double angulo)
    {
      matrizGlobal.AtribuirIdentidade();
      Ponto4D pontoPivo = bBox.obterCentro;

      matrizTmpTranslacao.AtribuirTranslacao(-pontoPivo.X, -pontoPivo.Y, -pontoPivo.Z); // Inverter sinal
      matrizGlobal = matrizTmpTranslacao.MultiplicarMatriz(matrizGlobal);

      RotacaoEixo(EixoRotacao.Z, angulo);
      matrizGlobal = matrizTmpRotacao.MultiplicarMatriz(matrizGlobal);

      matrizTmpTranslacaoInversa.AtribuirTranslacao(pontoPivo.X, pontoPivo.Y, pontoPivo.Z);
      matrizGlobal = matrizTmpTranslacaoInversa.MultiplicarMatriz(matrizGlobal);

      MatrizTransformacao = MatrizTransformacao.MultiplicarMatriz(matrizGlobal);
    }
    
    public void RotacaoEixo(EixoRotacao eixo, double angulo)
    {
      switch (eixo)
      {
        case EixoRotacao.X:
          matrizTmpRotacao.AtribuirRotacaoX(Transformacao4D.DEG_TO_RAD * angulo);
          break;
        case EixoRotacao.Y:
          matrizTmpRotacao.AtribuirRotacaoY(Transformacao4D.DEG_TO_RAD * angulo);
          break;
        case EixoRotacao.Z:
          matrizTmpRotacao.AtribuirRotacaoZ(Transformacao4D.DEG_TO_RAD * angulo);
          break;
      }
    }

    public void EscalaBBox(double sX, double sY, double sZ)
    {
      matrizGlobal.AtribuirIdentidade();
      Ponto4D pontoPivo = bBox.obterCentro;

      matrizTmpTranslacao.AtribuirTranslacao(-pontoPivo.X, -pontoPivo.Y, -pontoPivo.Z); // Inverter sinal
      matrizGlobal = matrizTmpTranslacao.MultiplicarMatriz(matrizGlobal);

      matrizTmpRotacao.AtribuirEscala(sX, sY, sZ);
      matrizGlobal = matrizTmpRotacao.MultiplicarMatriz(matrizGlobal);

      matrizTmpTranslacaoInversa.AtribuirTranslacao(pontoPivo.X, pontoPivo.Y, pontoPivo.Z);
      matrizGlobal = matrizTmpTranslacaoInversa.MultiplicarMatriz(matrizGlobal);

      MatrizTransformacao = MatrizTransformacao.MultiplicarMatriz(matrizGlobal);
    }
    
    private Transformacao4D AplicarRotacaoMatrizTemporaria(EixoRotacao eixoRotacao, double angulo)
    {
      var matrizTemporaria = MatrizTransformacaoTemporaria;
      switch (eixoRotacao)
      {
        case EixoRotacao.X:
          matrizTemporaria.AtribuirRotacaoX(Transformacao4D.DEG_TO_RAD * angulo);
          break;
        
        case EixoRotacao.Y:
          matrizTemporaria.AtribuirRotacaoY(Transformacao4D.DEG_TO_RAD * angulo);
          break;
        
        case EixoRotacao.Z:
          matrizTemporaria.AtribuirRotacaoZ(Transformacao4D.DEG_TO_RAD * angulo);
          break;
      }

      return matrizTemporaria;
    }
  }
}