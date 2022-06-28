using CG_Biblioteca;
using OpenTK;

namespace CG_N4
{
    public class ColisorEsfera : Colisor
    {
        private readonly float Raio;

        public ColisorEsfera(Esfera esfera) : this(objeto: esfera, raio: esfera.Raio)
        {
        }

        public ColisorEsfera(Objeto objeto, float raio) : base(objeto)
        {
            Raio = raio;
        }

        protected override bool ExisteColisaoPrecisa(Objeto outro)
        {
            Vector3 centro = Objeto.BBox.obterCentro.asVector3();
            Vector3 pontoMaisProximo = outro.Colisor.GetPontoMaisProximo(centro);
            float distancia = Matematica.Distancia(centro, pontoMaisProximo);
            return distancia <= Raio;
        }

        public override Vector3 GetPontoMaisProximo(Vector3 pontoOrigem)
        {
            Vector3 centro = Objeto.BBox.obterCentro.asVector3();
            Vector3 origem = pontoOrigem;

            Vector3 esferaPonto = Vector3.Normalize(origem - centro) * Raio;
            return centro + esferaPonto;
        }

        public override Vector3 GetCentroMassa()
        {
            return Objeto.BBox.obterCentro.asVector3();
        }
    }
}