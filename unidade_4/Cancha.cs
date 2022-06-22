using CG_Biblioteca;
using gcgcg;
using OpenTK.Graphics.OpenGL;

namespace CG_N3
{
    public class Cancha: Objeto
    {
        public Cancha(Ponto4D pontoCentral) : base(Utilitario.charProximo(), null)
        {
            FilhoAdicionar(new MuroCancha(new Ponto4D(0, 0, 0), 100, 600));
            FilhoAdicionar(new MuroCancha(new Ponto4D(300, 0, 0), 100, 1000));
        }

        protected override void DesenharGeometria()
        {
            
        }
    }

    class MuroCancha : ObjetoGeometria
    {
        public MuroCancha(Ponto4D ponto, int altura, int comprimento) : base('c', null)
        {
            PrimitivaTipo = PrimitiveType.Quads;
            ObjetoCor = new Cor(0, 255, 0);
            PontosAdicionar(ponto);
            PontosAdicionar(new Ponto4D(20, ponto.Y + altura, ponto.Z + comprimento));
        }

        protected override void DesenharObjeto()
        {
            GL.Begin(PrimitivaTipo);
            GL.Color4(ObjetoCor.CorR, ObjetoCor.CorG, ObjetoCor.CorB, ObjetoCor.CorA);
            foreach (var ponto in pontosLista)
            {
                GL.Vertex3(ponto.X, ponto.Y, ponto.Z);
            }
            GL.End();
        }
    }
}