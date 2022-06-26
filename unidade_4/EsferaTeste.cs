using System;
using OpenTK;

namespace CG_N4
{
    public class EsferaTeste : Esfera
    {
        public Vector3 Direcao { get; set; }

        public EsferaTeste(float raio, Vector3 direcao, uint stackCount = 32, uint sectorCount = 32)
            : base(raio, stackCount, sectorCount)
        {
            Direcao = direcao;
        }

        protected override void DesenharGeometria()
        {
            base.DesenharGeometria();
            BBox.Desenhar();
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            // 10cm/s
            float velocidade = (float)Utilitario.CentimetrosEmPixels(40);

            // qual a distância percorrida desde o último frame 
            float delta = (float)e.Time * velocidade;

            // calcula o deslocamento
            Vector3 deslocamento = Direcao * delta;

            // adiciona o deslocamento no objeto
            Translacao(deslocamento.X, deslocamento.Y, deslocamento.Z);
        }

        public override void OnColisao(Objeto outro)
        {
            Console.WriteLine(GetType() + "[" + Rotulo + "] - Colisão com o " + outro.GetType() + "[" + outro.Rotulo +
                              "]");
        }
    }
}