using System;
using OpenTK;

namespace CG_N4
{
    public class EsferaTeste : Esfera
    {
        public EsferaTeste(float raio, uint stackCount = 32, uint sectorCount = 32)
            : base(raio, stackCount, sectorCount)
        {
        }

        protected override void DesenharGeometria()
        {
            base.DesenharGeometria();
            BBox.Desenhar();
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            // calcula o deslocamento
            Vector3 deslocamento = ForcaFisica.Aceleracao * (float)e.Time;

            // adiciona o deslocamento no objeto
            Translacao(deslocamento.X, deslocamento.Y, deslocamento.Z);
        }

        public override void OnColisao(EventoColisao e)
        {
            Console.WriteLine(GetType() + "[" + Rotulo + "] - Colisão com o " + e.Objeto.GetType() + "[" + e.Objeto.Rotulo + "]");
            ForcaFisica.Aceleracao += e.ForcaFisica.Aceleracao;
        }
    }
}