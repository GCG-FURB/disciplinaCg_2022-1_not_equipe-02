using System;
using OpenTK;

namespace CG_N4
{
    public class ColisorChao : Colisor
    {
        private const float _coeficienteAtrito = 0.6f;

        // private const float _k1 = 1f; // coeficiente de atrito
        // private const float _k2 = _k1 * _k1; // coeficiente ao quadrao

        public ColisorChao(Objeto objeto) : base(objeto)
        {
        }

        protected override (Vector3 fA, Vector3 fB) ProcessarForcaColisao(FrameEventArgs e, Objeto objeto)
        {
            var ff = objeto.ForcaFisica;
            Vector3 v = ff.Velocidade;

            // Vector3 dragForce = v.Normalized();
            // dragForce = dragForce * _k1 + _k2 * dragForce * dragForce;
            // dragForce.Normalize();
            // // Vector3 fB = new Vector3(
            // //     Diminuir(e, v.X),
            // //     Diminuir(e, v.Y),
            // //     Diminuir(e, v.Z)
            // // );
            // return (Vector3.Zero, objeto.ForcaFisica.Velocidade - dragForce);

            float va = Math.Abs(v.X) + Math.Abs(v.Y) + Math.Abs(v.Z);
            if (Math.Abs(va) < (ff.Massa * _coeficienteAtrito) * 0.01)
            {
                return (Vector3.Zero, -v);
            }

            Vector3 fB = new Vector3(
                Diminuir(e, v.X),
                Diminuir(e, v.Y),
                Diminuir(e, v.Z)
            );
            return (Vector3.Zero, fB);
        }

        private float Diminuir(FrameEventArgs e, float f)
        {
            return f * -(_coeficienteAtrito * (float)e.Time);
        }

        protected override void AdicionarColisao(Objeto objeto)
        {
        }

        protected override bool ExisteColisaoPrecisa(Objeto objeto)
        {
            double menorY = objeto.BBox.obterMenorY;
            return menorY == 0;
        }

        public override Vector3 GetPontoMaisProximo(Vector3 origem)
        {
            return new Vector3(origem.X, 0, origem.Z);
        }

        public override Vector3 GetCentroMassa()
        {
            return Vector3.Zero;
        }
    }
}