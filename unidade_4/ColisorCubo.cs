using CG_Biblioteca;

namespace CG_N4
{
    public class ColisorCubo : Colisor
    {
        private const int MenorBBox = -1;
        private const int MaiorBBox = 1;
        private const int DentroBBox = 0;

        public ColisorCubo(Objeto objeto) : base(objeto)
        {
        }

        protected override bool ProcessarColisaoPrecisa(Objeto outro)
        {
            // se é colisor de Cubo, a BBox já é o suficiente!
            if (outro.Colisor.GetType().IsInstanceOfType(GetType()))
            {
                return true;
            }

            BBox bbox = Objeto.BBox;
            Ponto4D ponto = outro.Colisor.GetPontoMaisProximo(bbox.obterCentro);
            return (bbox.obterMenorX <= ponto.X && bbox.obterMaiorX >= ponto.X)
                   && (bbox.obterMenorY <= ponto.Y && bbox.obterMaiorY >= ponto.Y);
        }

        public override Ponto4D GetPontoMaisProximo(Ponto4D origem)
        {
            BBox bbox = Objeto.BBox;
            int x = bbox.obterMenorX > origem.X ? MenorBBox : (bbox.obterMaiorX < origem.X ? MaiorBBox : DentroBBox);
            int y = bbox.obterMenorY > origem.Y ? MenorBBox : (bbox.obterMaiorY < origem.Y ? MaiorBBox : DentroBBox);
            int z = bbox.obterMenorZ > origem.Z ? MenorBBox : (bbox.obterMaiorZ < origem.Z ? MaiorBBox : DentroBBox);
            return new Ponto4D(
                x == DentroBBox ? origem.X : (x == MaiorBBox ? bbox.obterMaiorX : bbox.obterMenorX),
                y == DentroBBox ? origem.Y : (x == MaiorBBox ? bbox.obterMaiorY : bbox.obterMenorY),
                z == DentroBBox ? origem.Z : (x == MaiorBBox ? bbox.obterMaiorZ : bbox.obterMenorZ)
            );
        }
    }
}