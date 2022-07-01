using System;
using OpenTK;

namespace CG_N4
{
    public class ColisorFundo : Colisor
    {

        private const float _coeficienteAreia = 10f;

        public ColisorFundo(Objeto objeto) : base(objeto)
        {
        }

        protected override (Vector3 fA, Vector3 fB) ProcessarForcaColisao(FrameEventArgs e, Objeto objeto)
        {
            Vector3 v = objeto.ForcaFisica.Velocidade;
            Vector3 fB = new Vector3(
                Diminuir(e, v.X),
                Diminuir(e, v.Y),
                Diminuir(e, v.Z)
            );
            return (Vector3.Zero, fB);
        }

        private float Diminuir(FrameEventArgs e, float f)
        {
            if (Math.Abs(f) < _coeficienteAreia)
            {
                return -f;
            }

            return f * -(_coeficienteAreia * (float)e.Time);
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