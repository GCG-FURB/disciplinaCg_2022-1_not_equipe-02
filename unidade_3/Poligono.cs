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

        public bool VerificarSeCliqueFoiDentro(Ponto4D pontoClique)
        {
            var pontos = pontosLista;
            int paridade = 0;
            for (int i = 0; i < pontos.Count; i++)
            {
                var proximoIndexComparacao = i + 1;
                if (proximoIndexComparacao == pontos.Count)
                {
                    proximoIndexComparacao = 0;
                }
                
                var primeiroPontoComparacao = pontos[i];
                var segundoPontoComparacao = pontos[proximoIndexComparacao];

                var ti = Matematica.InterseccaoScanLine(pontoClique.Y, primeiroPontoComparacao.Y, segundoPontoComparacao.Y);
                if (ti >= 0 && ti <= 1)
                {
                    var xi = Matematica.CalculaXiScanLine(primeiroPontoComparacao.X, segundoPontoComparacao.X, ti);
                    if (xi > pontoClique.X)
                    {
                        paridade++;
                    }
                }
            }

            if (paridade % 2 == 0)
            {
                return false;
            }

            return true;
        }

        public ReadOnlyCollection<Ponto4D> ObterPontos() => pontosLista.AsReadOnly();
    }
}