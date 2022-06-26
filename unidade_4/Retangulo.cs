/**
  Autor: Dalton Solano dos Reis
**/

using CG_Biblioteca;

namespace CG_N4
{
    internal class Retangulo : ObjetoGeometria
    {
        public Retangulo(char rotulo, Objeto paiRef, Ponto4D ptoInfEsq, Ponto4D ptoSupDir) : base(rotulo, paiRef)
        {
            PontosAdicionar(ptoInfEsq);
            PontosAdicionar(new Ponto4D(ptoSupDir.X, ptoInfEsq.Y));
            PontosAdicionar(ptoSupDir);
            PontosAdicionar(new Ponto4D(ptoInfEsq.X, ptoSupDir.Y));
        }

        //TODO: melhorar para exibir não só a lista de pontos (geometria), mas também a topologia ... poderia ser listado estilo OBJ da Wavefrom
        public override string ToString()
        {
            string retorno;
            retorno = "__ Objeto Retangulo: " + Rotulo + "\n";
            for (var i = 0; i < Pontos.Count; i++)
            {
                retorno += "P" + i + "[" + Pontos[i].X + "," + Pontos[i].Y + "," + Pontos[i].Z + "," + Pontos[i].W +
                           "]" + "\n";
            }

            return (retorno);
        }
    }
}