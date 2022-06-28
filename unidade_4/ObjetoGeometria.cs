/**
  Autor: Dalton Solano dos Reis
**/

using System.Collections.Generic;
using CG_Biblioteca;
using OpenTK.Graphics.OpenGL;

namespace CG_N4
{
    public abstract class ObjetoGeometria : Objeto
    {
        protected List<Ponto4D> Pontos = new List<Ponto4D>();

        public ObjetoGeometria(char rotulo, Objeto paiRef) : base(rotulo, paiRef)
        {
        }

        protected override void DesenharGeometria()
        {
            GL.Begin(PrimitivaTipo);

            foreach (var ponto in Pontos)
            {
                GL.Vertex3(ponto.X, ponto.Y, ponto.Z);
            }

            GL.End();
        }

        public void PontosAdicionar(Ponto4D pto)
        {
            Pontos.Add(pto);
            if (Pontos.Count.Equals(1))
            {
                BBox.Atribuir(pto);
            }
            else
            {
                BBox.Atualizar(pto);
            }

            BBox.ProcessarCentro();
        }

        public void PontosRemoverUltimo()
        {
            Pontos.RemoveAt(Pontos.Count - 1);
        }

        protected void PontosRemoverTodos()
        {
            Pontos.Clear();
        }

        public Ponto4D PontosUltimo()
        {
            return Pontos[Pontos.Count - 1];
        }

        public void PontosRemover(int index)
        {
            Pontos.RemoveAt(index);
        }

        public Objeto GetObjetoDentro(Ponto4D coordenada)
        {
            // var estaDentroBBox = VerificarSeCoordenadaEstaDentroBBox(coordenada);
            // if (estaDentroBBox)
            // {
            //     // if (ExecutarScanline(coordenada))
            //     // {
            //     //   return (true, this);
            //     // }
            // }
            //
            // // foreach (ObjetoGeometria objetoGeometria in ObterObjetosFilhos())
            // // {
            // //   var verificacaoEstaDentroDeUmFilho = objetoGeometria.VerificarSeCoordenadaEstaDentro(coordenada);
            // //   if (verificacaoEstaDentroDeUmFilho.EstaDentro)
            // //   {
            // //     return verificacaoEstaDentroDeUmFilho;
            // //   }
            // // }

            return null;
        }

        // public (double menorDistancia, Poligono poligonoMenorDistancia, Ponto4D coordenadaMenorDistancia, int indexPontoMenorDistancia) ObterVerticeMaisProximo(Ponto4D pontoCoordenadaMouse, (double menorDistancia, Poligono poligonoMenorDistancia, Ponto4D coordenadaMenorDistancia, int indexPontoMenorDistancia) resultadoCalculo)
        // {
        //    
        //    var pontosPoligono = pontosLista;
        //    for (var i = 0; i < pontosPoligono.Count; i++)
        //    {
        //      var distancia = Matematica.Distancia(pontoCoordenadaMouse, pontosPoligono[i]);
        //      if (distancia <= resultadoCalculo.menorDistancia)
        //      {
        //        resultadoCalculo.menorDistancia = distancia;
        //        resultadoCalculo.poligonoMenorDistancia = this as Poligono;
        //        resultadoCalculo.coordenadaMenorDistancia = pontosPoligono[i];
        //        resultadoCalculo.indexPontoMenorDistancia = i;
        //      }
        //    }
        //   
        //    var objetosFilhos = ObterObjetosFilhos();
        //    foreach (Poligono poligonoFilho in objetosFilhos)
        //    {
        //      if (poligonoFilho != null)
        //      {
        //        resultadoCalculo = poligonoFilho.ObterVerticeMaisProximo(pontoCoordenadaMouse, resultadoCalculo);
        //      }
        //    }
        //
        //    return resultadoCalculo;
        // }

        public override string ToString()
        {
            string retorno = "__ Objeto: " + Rotulo + "\n";
            for (var i = 0; i < Pontos.Count; i++)
            {
                retorno += "P" + i +
                           "[" + Pontos[i].X + "," + Pontos[i].Y + "," + Pontos[i].Z + "," + Pontos[i].W + "]" + "\n";
            }

            return retorno;
        }

        // private bool ExecutarScanline(Ponto4D coordenada)
        // {
        //   var pontos = pontosLista;
        //   int paridade = 0;
        //   for (int i = 0; i < pontos.Count; i++)
        //   {
        //     var proximoIndexComparacao = i + 1;
        //     if (proximoIndexComparacao == pontos.Count)
        //     {
        //       proximoIndexComparacao = 0;
        //     }
        //             
        //     var primeiroPontoComparacao = pontos[i];
        //     var segundoPontoComparacao = pontos[proximoIndexComparacao];
        //
        //     var ti = Matematica.InterseccaoScanLine(coordenada.Y, primeiroPontoComparacao.Y, segundoPontoComparacao.Y);
        //     if (ti >= 0 && ti <= 1)
        //     {
        //       var xi = Matematica.CalculaXiScanLine(primeiroPontoComparacao.X, segundoPontoComparacao.X, ti);
        //       if (xi > coordenada.X)
        //       {
        //         paridade++;
        //       }
        //     }
        //   }
        //
        //   return (paridade % 2 > 0);
        // }
    }
}