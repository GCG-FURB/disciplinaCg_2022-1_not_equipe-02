using System;
using System.Numerics;
using CG_Biblioteca;
using CG_N4;

namespace CG_N4
{
    public class ColisorBBox : Colisor
    {
        private const int MenorBBox = -1;
        private const int MaiorBBox = 1;
        private const int DentroBBox = 0;
        
        public ColisorBBox(Objeto objeto) : this(objeto, 0)
        {
        }
        
        protected ColisorBBox(Objeto objeto, int prioridade) : base(objeto, prioridade)
        {
        }

        protected override bool ProcessarColisao(Objeto objeto)
        {
            BBox a = Objeto.BBox;
            BBox b = objeto.BBox;
            // Intersecção
            return (a.obterMenorX <= b.obterMaiorX && a.obterMaiorX >= b.obterMenorX) // 
                   && a.obterMenorY <= b.obterMaiorY && a.obterMaiorY >= b.obterMenorY //
                   && a.obterMenorZ <= b.obterMaiorZ && a.obterMaiorZ >= b.obterMenorZ;
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