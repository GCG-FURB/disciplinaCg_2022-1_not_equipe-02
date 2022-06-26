using CG_Biblioteca;
using OpenTK.Graphics.OpenGL;

namespace CG_N4
{
    public class Cancha : Objeto
    {
        private Ponto4D PontoInicial;
        private double Largura;
        private double Comprimento;
        private double Altura;

        public Cancha(Ponto4D pontoInicial, double largura, double comprimento, double altura)
            : base(Utilitario.charProximo(), null)
        {
            PontoInicial = pontoInicial;
            Largura = largura;
            Comprimento = comprimento;
            Altura = altura;

            FilhoAdicionar(new MuroCancha(PontoInicial, Altura, Comprimento, 20, true));
            FilhoAdicionar(new MuroCancha(PontoInicial + new Ponto4D(0, 0, Largura), Altura, Comprimento, 20, false));
            FilhoAdicionar(new ChaoCancha(PontoInicial, Largura, Comprimento));
            FilhoAdicionar(new FundoCancha(PontoInicial + new Ponto4D(Comprimento), Largura, Altura));
        }

        protected override void DesenharGeometria()
        {
        }
    }

    class MuroCancha : Objeto
    {
        public readonly double Altura;
        public readonly double Comprimento;
        public readonly double Largura;
        public readonly bool Esquerda;
        public readonly Ponto4D PontoInicial;

        public MuroCancha(Ponto4D ponto, double altura, double comprimento, double largura, bool esquerda)
            : base('c', null)
        {
            PrimitivaTipo = PrimitiveType.Quads;
            ObjetoCor = new Cor(0);
            PrimitivaTamanho = 1;
            Colisor = new ColisorCubo(this);

            Altura = altura;
            Comprimento = comprimento;
            Largura = largura;
            Esquerda = esquerda;
            PontoInicial = ponto;
            
            BBox.Atribuir(ponto);
            BBox.Atualizar(ponto + new Ponto4D(comprimento, altura, Esquerda ? largura * -1 : largura));
            BBox.ProcessarCentro();
        }

        protected override void DesenharGeometria()
        {
            double x = PontoInicial.X;
            double y = PontoInicial.Y;
            double z = PontoInicial.Z;

            double a = Altura;
            double c = Comprimento;
            double l = Largura;

            GL.Begin(PrimitivaTipo);

            // TODO: Ver com o Dalton pq a ordem das batatas altera a maionese
            // face interna do muro
            if (Esquerda)
            {
                GL.Normal3(0, 0, -1);
                GL.Vertex3(x, y + a, z);
                GL.Vertex3(x, y, z);
                GL.Vertex3(x + c, y, z);
                GL.Vertex3(x + c, y + a, z);          
            }
            else
            {
                GL.Normal3(0, 0, 1);
                GL.Vertex3(x, y, z);
                GL.Vertex3(x, y + a, z);
                GL.Vertex3(x + c, y + a, z);
                GL.Vertex3(x + c, y, z);
            }
            

            z -= Esquerda ? l : 0;
            // face da frente
            GL.Normal3(-1, 0, 0);
            GL.Vertex3(x, y, z);
            GL.Vertex3(x, y, z + l);
            GL.Vertex3(x, y + a, z + l);
            GL.Vertex3(x, y + a, z);

            // face de cima
            GL.Normal3(0, 1, 0);
            GL.Vertex3(x, y + a, z);
            GL.Vertex3(x, y + a, z + l);
            GL.Vertex3(x + c, y + a, z + l);
            GL.Vertex3(x + c, y + a, z);

            GL.End();
        }
    }

    class ChaoCancha : ObjetoGeometria
    {
        public ChaoCancha(Ponto4D ponto, double largura, double comprimento) 
            : base('c', null)
        {
            PrimitivaTipo = PrimitiveType.Quads;
            ObjetoCor = new Cor(200, 200, 0);
            PrimitivaTamanho = 1;

            PontosAdicionar(ponto);
            PontosAdicionar(ponto + new Ponto4D(0, 0, largura));
            PontosAdicionar(ponto + new Ponto4D(comprimento, 0, largura));
            PontosAdicionar(ponto + new Ponto4D(comprimento));
        }
    }

    class FundoCancha : ObjetoGeometria
    {
        public FundoCancha(Ponto4D ponto, double largura, double altura) 
            : base('c', null)
        {
            PrimitivaTipo = PrimitiveType.Quads;
            ObjetoCor = new Cor(255, 0, 0);
            PrimitivaTamanho = 1;
            Colisor = new ColisorCubo(this);

            PontosAdicionar(ponto);
            PontosAdicionar(ponto + new Ponto4D(0, 0, largura));
            PontosAdicionar(ponto + new Ponto4D(0, altura, largura));
            PontosAdicionar(ponto + new Ponto4D(0, altura));
            
            BBox.Atribuir(ponto);
            BBox.Atualizar(ponto + new Ponto4D(0, altura, largura));
            BBox.ProcessarCentro();
        }
    }
}