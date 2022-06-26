using System;
using System.Collections.Generic;
using CG_Biblioteca;

namespace CG_N4
{
    public abstract class Colisor
    {
        public readonly Objeto Objeto;
        public readonly List<Objeto> Colisoes = new List<Objeto>();

        protected Colisor(Objeto objeto)
        {
            if (objeto == null)
            {
                throw new ArgumentException("Objeto obrigatório");
            }

            Objeto = objeto;
        }

        public bool ExisteColisao(Objeto objeto)
        {
            if (objeto?.Colisor == null)
            {
                return false;
            }

            if (!ExisteColisaoBBox(objeto))
            {
                return false;
            }

            return ProcessarColisaoPrecisa(objeto);
        }

        protected abstract bool ProcessarColisaoPrecisa(Objeto outro);

        public abstract Ponto4D GetPontoMaisProximo(Ponto4D origem);
        
        protected bool ExisteColisaoBBox(Objeto objeto)
        {
            BBox a = Objeto.BBox;
            BBox b = objeto.BBox;
            // Intersecção
            return (a.obterMenorX <= b.obterMaiorX && a.obterMaiorX >= b.obterMenorX) // 
                   && a.obterMenorY <= b.obterMaiorY && a.obterMaiorY >= b.obterMenorY //
                   && a.obterMenorZ <= b.obterMaiorZ && a.obterMaiorZ >= b.obterMenorZ;
        }

    }
}