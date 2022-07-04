using CG_Biblioteca;
using OpenTK;
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
            FilhoAdicionar(new LinhaJogada(PontoInicial + new Ponto4D(Utilitario.MetrosEmPixels(1d)), Largura));
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
            ObjetoCor = new Cor(222, 184, 135);
            PrimitivaTamanho = 1;
            Colisor = new ColisorCubo(this);
            ForcaFisica.Massa = 2 * 1000 * 1000 * 1000;

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
            DesenharMuro();
            DesenharLinhas();
        }

        private void DesenharLinhas()
        {
            double x = PontoInicial.X;
            double y = PontoInicial.Y;
            double z = PontoInicial.Z;

            double a = Altura;
            double c = Comprimento;
            double l = Largura;

            GL.Color3(0, 0, 0);

            GL.Begin(PrimitiveType.LineLoop);
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

            GL.End();
            GL.Begin(PrimitiveType.LineLoop);

            z -= Esquerda ? l : 0;
            // face da frente
            GL.Normal3(-1, 0, 0);
            GL.Vertex3(x, y, z);
            GL.Vertex3(x, y, z + l);
            GL.Vertex3(x, y + a, z + l);
            GL.Vertex3(x, y + a, z);

            GL.End();
            GL.Begin(PrimitiveType.LineLoop);

            // face de cima
            GL.Normal3(0, 1, 0);
            GL.Vertex3(x, y + a, z);
            GL.Vertex3(x, y + a, z + l);
            GL.Vertex3(x + c, y + a, z + l);
            GL.Vertex3(x + c, y + a, z);
            GL.End();
        }

        private void DesenharMuro()
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

        public override void OnColisao(EventoColisao e)
        {
            base.OnColisao(e);
            ForcaFisica.Aceleracao = Vector3.Zero;
        }
    }

    class ChaoCancha : ObjetoGeometria
    {
        public readonly double Largura;
        public readonly double Comprimento;

        public ChaoCancha(Ponto4D ponto, double largura, double comprimento)
            : base('c', null)
        {
            Largura = largura;
            Comprimento = comprimento;

            PrimitivaTipo = PrimitiveType.Quads;
            PrimitivaTamanho = 1;
            Colisor = new ColisorChao(this);
            Textura = TexturaImagem.FromResources("CG_N4.Resources.sand.jpeg");

            PontosAdicionar(ponto + new Ponto4D(0, 0, largura));
            PontosAdicionar(ponto + new Ponto4D(comprimento, 0, largura));
            PontosAdicionar(ponto + new Ponto4D(comprimento));
            PontosAdicionar(ponto);
        }

        protected override void DesenharGeometria()
        {
            float s = 20.0f;
            float t = s * (float)(Largura / Comprimento);

            GL.Begin(PrimitivaTipo);

            GL.Normal3(0.0f, 1.0f, 0.0f);

            GL.TexCoord2(0.0f, t);
            Pontos[0].Desenhar();

            GL.TexCoord2(s, t);
            Pontos[1].Desenhar();

            GL.TexCoord2(s, 0.0f);
            Pontos[2].Desenhar();

            GL.TexCoord2(0.0f, 0.0f);
            Pontos[3].Desenhar();

            GL.End();
        }
    }

    class FundoCancha : ObjetoGeometria
    {
        public FundoCancha(Ponto4D ponto, double largura, double altura)
            : base('c', null)
        {
            PrimitivaTipo = PrimitiveType.Quads;
            ObjetoCor = new Cor(222, 184, 135);
            PrimitivaTamanho = 1;
            Colisor = new ColisorCubo(this);
            ForcaFisica.Massa = 2 * 1000 * 1000 * 1000;

            PontosAdicionar(ponto);
            PontosAdicionar(ponto + new Ponto4D(0, 0, largura));
            PontosAdicionar(ponto + new Ponto4D(0, altura, largura));
            PontosAdicionar(ponto + new Ponto4D(0, altura));

            BBox.Atribuir(ponto);
            BBox.Atualizar(ponto + new Ponto4D(200, altura, largura));
            BBox.ProcessarCentro();
        }

        public override void OnColisao(EventoColisao e)
        {
            base.OnColisao(e);
            ForcaFisica.Aceleracao = Vector3.Zero;
        }
    }

    class LinhaJogada : ObjetoGeometria
    {
        public readonly double Largura;
        public readonly double Comprimento = 20.0D;

        public LinhaJogada(Ponto4D ponto, double largura) : base(Utilitario.charProximo(), null)
        {
            Largura = largura;

            PrimitivaTipo = PrimitiveType.Quads;
            PrimitivaTamanho = 1;
            Textura = TexturaImagem.FromResources("CG_N4.Resources.sand.jpeg");
            ObjetoCor = new Cor(255, 0, 0);

            PontosAdicionar(ponto + new Ponto4D(0, 1d));
            PontosAdicionar(ponto + new Ponto4D(0, 1d, largura));
            PontosAdicionar(ponto + new Ponto4D(Utilitario.CentimetrosEmPixels(5d), 1d, largura));
            PontosAdicionar(ponto + new Ponto4D(Utilitario.CentimetrosEmPixels(5d), 1d));
        }

        protected override void DesenharGeometria()
        {
            float s = 20.0f;
            float t = s * (float)(Largura / Comprimento);

            GL.Begin(PrimitivaTipo);

            GL.Normal3(0.0f, 1.0f, 0.0f);

            GL.TexCoord2(0.0f, t);
            Pontos[0].Desenhar();

            GL.TexCoord2(s, t);
            Pontos[1].Desenhar();

            GL.TexCoord2(s, 0.0f);
            Pontos[2].Desenhar();

            GL.TexCoord2(0.0f, 0.0f);
            Pontos[3].Desenhar();

            GL.End();
        }
    }
}