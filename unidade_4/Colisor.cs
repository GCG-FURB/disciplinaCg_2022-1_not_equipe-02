using System;
using System.Collections.Generic;
using CG_Biblioteca;
using OpenTK;

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

        public void ProcessarColisao(FrameEventArgs e, Objeto objeto)
        {
            if (objeto?.Colisor == null || !ExisteColisaoBBox(objeto) || !ExisteColisaoPrecisa(objeto))
            {
                RemoverColisao(objeto);
                return;
            }

            if (Colisoes.Contains(objeto))
            {
                return;
            }

            (Vector3 fA, Vector3 fB) = ProcessarForcaColisao(e, objeto);

            Objeto.ForcaFisica.Aceleracao += fA;
            objeto.ForcaFisica.Aceleracao += fB;
            
            Objeto.OnColisao(new EventoColisao(objeto, fA));
            objeto.OnColisao(new EventoColisao(Objeto, fB));

            AdicionarColisao(objeto);
        }

        protected virtual (Vector3 fA, Vector3 fB) ProcessarForcaColisao(FrameEventArgs e, Objeto objeto)
        {
            Vector3 cmA;
            Vector3 cmB;
            if (Objeto.ForcaFisica.Velocidade != Vector3.Zero)
            {
                cmA = GetCentroMassa();
                cmB = objeto.Colisor.GetPontoMaisProximo(cmA);
            }
            else
            {
                cmB = objeto.Colisor.GetCentroMassa();
                cmA = GetPontoMaisProximo(cmB);
            }

            Vector3 vA = Objeto.ForcaFisica.Velocidade;
            Vector3 vB = objeto.ForcaFisica.Velocidade;

            float massaA = Objeto.ForcaFisica.Massa;
            float massaB = objeto.ForcaFisica.Massa;

            float d = Matematica.Distancia(cmA, cmB);

            // normal
            Vector3 n = (cmB - cmA) / d;
            if (float.IsNaN(n.X))
            {
                n = Vector3.Zero;
            }

            // tangencia
            Vector3 t = new Vector3(-n.Z, n.Y, n.X);

            // produto tangente
            float dpTanA = (vA.X * t.X) + (vA.Y * t.Y) + (vA.Z * t.Z);
            float dpTanB = (vB.X * t.X) + (vB.Y * t.Y) + (vB.Z * t.Z);

            // produto normal
            float dpNormA = (vA.X * n.X) + (vA.Y * n.Y) + (vA.Z * n.Z);
            float dpNormB = (vB.X * n.X) + (vB.Y * n.Y) + (vB.Z * n.Z);

            // momento linear
            float momentoA = (dpNormA * (massaA - massaB) + 2.0f * massaB * dpNormB) / (massaA + massaB);
            float momentoB = (dpNormB * (massaB - massaA) + 2.0f * massaA * dpNormA) / (massaA + massaB);

            // aceleração
            Vector3 fA = Matematica.Arredondar(new Vector3(
                t.X * dpTanA + n.X * momentoA,
                t.Y * dpTanA + n.Y * momentoA,
                t.Z * dpTanA + n.Z * momentoA
            ) - vA);
            Vector3 fB = Matematica.Arredondar(new Vector3(
                t.X * dpTanB + n.X * momentoB,
                t.Y * dpTanB + n.Y * momentoB,
                t.Z * dpTanB + n.Z * momentoB
            ) - vB);
            return (fA, fB);
        }

        protected virtual void AdicionarColisao(Objeto objeto)
        {
            objeto.Colisor.Colisoes.Add(objeto);
            Colisoes.Add(objeto);
        }

        private void RemoverColisao(Objeto objeto)
        {
            if (Colisoes.Contains(objeto))
            {
                objeto.Colisor.Colisoes.Remove(Objeto);
                Colisoes.Remove(objeto);
            }
        }

        protected abstract bool ExisteColisaoPrecisa(Objeto objeto);

        // protected abstract bool ProcessarColisaoInner(Objeto outro);

        public abstract Vector3 GetPontoMaisProximo(Vector3 origem);

        public abstract Vector3 GetCentroMassa();
        
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