using System;
using OpenTK;

namespace CG_N4
{
    public class ForcaFisica
    {
        public readonly Objeto Objeto;
        public float Massa;
        public Vector3 Aceleracao;
        public Vector3 Velocidade;

        public ForcaFisica(Objeto objeto)
            : this(objeto: objeto, massa: 1.0f, aceleracao: Vector3.Zero, velocidade: Vector3.Zero)
        {
        }

        public ForcaFisica(Objeto objeto, float massa, Vector3 aceleracao, Vector3 velocidade)
        {
            Objeto = objeto;
            Massa = massa;
            Aceleracao = aceleracao;
            Velocidade = velocidade;
        }

        public override string ToString()
        {
            return "ForcaFisica[" + Objeto.GetType() + "[" + Objeto.Rotulo + "]] "
                   + "- Massa: " + MassaToString()
                   + ", Velocidade: " + Velocidade
                   + ", Aceleracao: " + Aceleracao;
        }

        private string MassaToString()
        {
            // KT
            if (Massa >= 1 * 1000 * 1000 * 1000)
            {
                return Math.Round(Massa / (1 * 1000 * 1000 * 1000), 3) + "KT";
            }
            
            // T
            if (Massa >= 1 * 1000 * 1000)
            {
                return Math.Round(Massa / (1 * 1000 * 1000), 3) + "T";
            }
            
            // KG
            if (Massa >= 1 * 1000)
            {
                return Math.Round(Massa / (1 * 1000), 3) + "Kg";
            }

            return Massa + "g";
        }
    }
}