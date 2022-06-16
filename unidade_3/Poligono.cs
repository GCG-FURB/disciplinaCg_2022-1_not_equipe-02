using System.Collections.ObjectModel;
using CG_Biblioteca;
using OpenTK.Graphics.OpenGL;

namespace gcgcg
{
    public class Poligono : ObjetoGeometria
    {
        public Poligono(char rotulo, Objeto paiRef) : base(rotulo, paiRef)
        {
            PrimitivaTipo = PrimitiveType.LineLoop;
            ObjetoCor = new Cor(255, 255, 255);
        }

        protected override void DesenharObjeto()
        {
            GL.Begin(PrimitivaTipo);

            foreach (var ponto in pontosLista)
            {
                GL.Vertex2(ponto.X, ponto.Y);
            }
            
            GL.End();
        }

        public void ModificarCoordenadaUltimoPonto(double mouseX, double mouseY)
        {
            PontosUltimo().X = mouseX;
            PontosUltimo().Y = mouseY;
        }

        

        public ReadOnlyCollection<Ponto4D> ObterPontos() => pontosLista.AsReadOnly();

        public void AlternarPrimitivaTipoEntreLineStripELineLoop()
        {
            if (PrimitivaTipo == PrimitiveType.LineLoop)
            {
                PrimitivaTipo = PrimitiveType.LineStrip;
                return;
            }
            
            if (PrimitivaTipo == PrimitiveType.LineStrip)
            {
                PrimitivaTipo = PrimitiveType.LineLoop;
                return;
            }
        }

       
    }
}
