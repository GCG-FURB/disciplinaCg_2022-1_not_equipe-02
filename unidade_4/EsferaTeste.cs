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
    }
}