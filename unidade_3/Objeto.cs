/**
  Autor: Dalton Solano dos Reis
**/

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

    private Transformacao4D MatrizTransformacaoTemporaria = new Transformacao4D();
    private Transformacao4D MatrizTransformacao =  new Transformacao4D();

    public Objeto(char rotulo, Objeto paiRef)
    {
      this.rotulo = rotulo;
      Rotulo = rotulo;
    }

    public void Desenhar()
    {
      GL.PushMatrix();                                    // N3-Exe12: grafo de cena
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