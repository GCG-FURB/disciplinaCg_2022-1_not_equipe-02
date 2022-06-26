using OpenTK;

namespace CG_N4
{
    public class EsferaTeste : Esfera
    {
        public EsferaTeste(float raio, uint stackCount = 32, uint sectorCount = 32)
            : base(raio, stackCount, sectorCount)
        {
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            // 10cm/s
            float velocidade = (float) Utilitario.CentimetrosEmPixels(10);
            
            // qual a distância percorrida desde o último frame 
            float delta = (float)e.Time * velocidade;
            
            // vetor de direção
            Vector3 direcao = new Vector3(1, 0, 0);
            
            // calcula o deslocamento
            Vector3 deslocamento = direcao * delta;
            
            // adiciona o deslocamento no objeto
            Translacao(deslocamento.X, deslocamento.Y, deslocamento.Z);
        }
    }
}