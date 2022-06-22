using CG_Biblioteca;
using gcgcg;
using OpenTK.Graphics.OpenGL;

namespace CG_N3
{
    public class Cancha : Objeto
    {
        private Ponto4D PontoInicial;
        private double Largura;
        private double Comprimento;
        private double Altura;

        public Cancha(Ponto4D pontoInicial, double largura, double comprimento, double altura) : base(
            Utilitario.charProximo(),
            null)
        {
            PontoInicial = pontoInicial;
            Largura = largura;
            Comprimento = comprimento;
            Altura = altura;

            FilhoAdicionar(new MuroCancha(PontoInicial, Altura, Comprimento, -20));
            FilhoAdicionar(new MuroCancha(PontoInicial + new Ponto4D(0, 0, Largura), Altura, Comprimento, 20));
            FilhoAdicionar(new ChaoCancha(PontoInicial, Largura, Comprimento));
        }

        protected override void DesenharGeometria()
        {
        }
    }

    class MuroCancha : ObjetoGeometria
    {
        public MuroCancha(Ponto4D ponto, double altura, double comprimento, double largura) : base('c', null)
        {
            PrimitivaTipo = PrimitiveType.Quads;
            ObjetoCor = new Cor(0);
            PrimitivaTamanho = 1;

            // face interna do muro
            PontosAdicionar(ponto);
            PontosAdicionar(ponto + new Ponto4D(0, altura));
            PontosAdicionar(ponto + new Ponto4D(comprimento, altura));
            PontosAdicionar(ponto + new Ponto4D(comprimento));

            // face da frente
            PontosAdicionar(ponto);
            PontosAdicionar(ponto + new Ponto4D(0, 0, largura));
            PontosAdicionar(ponto + new Ponto4D(0, altura, largura));
            PontosAdicionar(ponto + new Ponto4D(0, altura));

            // face de cima
            PontosAdicionar(ponto + new Ponto4D(0, altura));
            PontosAdicionar(ponto + new Ponto4D(0, altura, largura));
            PontosAdicionar(ponto + new Ponto4D(comprimento, altura, largura));
            PontosAdicionar(ponto + new Ponto4D(comprimento, altura));
        }

        protected override void DesenharObjeto()
        {
            GL.Begin(PrimitivaTipo);

            foreach (var ponto in pontosLista)
            {
                GL.Vertex3(ponto.X, ponto.Y, ponto.Z);
            }

            GL.End();
        }
    }

    class ChaoCancha : ObjetoGeometria
    {
        public ChaoCancha(Ponto4D ponto, double largura, double comprimento) : base('c', null)
        {
            PrimitivaTipo = PrimitiveType.Quads;
            ObjetoCor = new Cor(255, 255, 0);
            PrimitivaTamanho = 1;

            PontosAdicionar(ponto);
            PontosAdicionar(ponto + new Ponto4D(0, 0, largura));
            PontosAdicionar(ponto + new Ponto4D(comprimento, 0, largura));
            PontosAdicionar(ponto + new Ponto4D(comprimento));
        }

        protected override void DesenharObjeto()
        {
            GL.Begin(PrimitivaTipo);

            foreach (var ponto in pontosLista)
            {
                GL.Vertex3(ponto.X, ponto.Y, ponto.Z);
            }

            GL.End();
        }
    }
}