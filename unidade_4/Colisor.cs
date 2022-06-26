using System;
using System.Collections.Generic;
using System.Numerics;
using CG_Biblioteca;

namespace CG_N4
{
    public abstract class Colisor
    {
        public readonly Objeto Objeto;
        public readonly List<Objeto> Colisoes = new List<Objeto>();
        public readonly int Prioridade;

        protected Colisor(Objeto objeto, int prioridade)
        {
            if (objeto == null)
            {
                throw new ArgumentException("Objeto obrigatório");
            }

            Objeto = objeto;
            Prioridade = prioridade;
        }

        public bool ExisteColisao(Objeto objeto)
        {
            if (objeto?.Colisor == null)
            {
                return false;
            }
            return ProcessarColisao(objeto);
        }

        protected abstract bool ProcessarColisao(Objeto objeto);

        public abstract Ponto4D GetPontoMaisProximo(Ponto4D origem);

    }
}