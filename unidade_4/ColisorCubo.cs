using CG_Biblioteca;
using OpenTK;

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

        protected override bool ExisteColisaoPrecisa(Objeto outro)
        {
            // se é colisor de Cubo, a BBox já é o suficiente!
            if (outro.Colisor.GetType().IsInstanceOfType(GetType()))
            {
                return true;
            }

            Vector3 ponto = GetPontoMaisProximo(outro.BBox.obterCentro.asVector3());
            BBox bbox = Objeto.BBox;
            return (bbox.obterMenorX <= ponto.X && bbox.obterMaiorX >= ponto.X)
                   && (bbox.obterMenorY <= ponto.Y && bbox.obterMaiorY >= ponto.Y)
                   && (bbox.obterMenorZ <= ponto.Z && bbox.obterMaiorZ >= ponto.Z);
        }

        public override Vector3 GetPontoMaisProximo(Vector3 origem)
        {
            BBox bbox = Objeto.BBox;
            int x = bbox.obterMenorX > origem.X ? MenorBBox : (bbox.obterMaiorX < origem.X ? MaiorBBox : DentroBBox);
            int y = bbox.obterMenorY > origem.Y ? MenorBBox : (bbox.obterMaiorY < origem.Y ? MaiorBBox : DentroBBox);
            int z = bbox.obterMenorZ > origem.Z ? MenorBBox : (bbox.obterMaiorZ < origem.Z ? MaiorBBox : DentroBBox);
            return new Vector3(
                x == DentroBBox ? origem.X : (float)(x == MaiorBBox ? bbox.obterMaiorX : bbox.obterMenorX),
                y == DentroBBox ? origem.Y : (float)(x == MaiorBBox ? bbox.obterMaiorY : bbox.obterMenorY),
                z == DentroBBox ? origem.Z : (float)(x == MaiorBBox ? bbox.obterMaiorZ : bbox.obterMenorZ)
            );
        }

        public override Vector3 GetCentroMassa()
        {
            return Objeto.BBox.obterCentro.asVector3();
        }
    }
}