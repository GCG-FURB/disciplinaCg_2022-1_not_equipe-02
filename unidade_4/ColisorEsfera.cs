using System;
using CG_Biblioteca;
using OpenTK;

namespace CG_N4
{
    public class ColisorEsfera : ColisorBBox
    {
        private readonly float Raio;

        public ColisorEsfera(Esfera esfera) : this(objeto: esfera, raio: esfera.Raio)
        {
            
        }
        
        public ColisorEsfera(Objeto objeto, float raio) : base(objeto, 1)
        {
            Raio = raio;
        }

        protected override bool ProcessarColisao(Objeto objeto)
        {
            if (base.ProcessarColisao(objeto))
            {
                Ponto4D centro = Objeto.BBox.obterCentro;

                Ponto4D pontoMaisProximo = objeto.Colisor.GetPontoMaisProximo(centro);
                double distancia = Matematica.Distancia(centro, pontoMaisProximo);
                Console.WriteLine(GetType() + " - " + Objeto.GetType()+ "[" + Objeto.Rotulo + "] -> " + objeto.GetType() + "[" + objeto.Rotulo + "] -  Distância: " + distancia + " - " + (distancia <= Raio ? "Colisão por raio" : "Sem colisão por raio"));
                return distancia <= Raio;
            }

            Console.WriteLine(GetType() + " - " + Objeto.GetType()+ "[" + Objeto.Rotulo + "] -> " + objeto.GetType() + "[" + objeto.Rotulo + "] -  Sem colisão por BBOX");

            return false;
        }

        public override Ponto4D GetPontoMaisProximo(Ponto4D pontoOrigem)
        {
            Vector3 centro = Objeto.BBox.obterCentro.asVector3();
            Vector3 origem = pontoOrigem.asVector3();

            Vector3 esferaPonto = Vector3.Normalize(origem - centro) * Raio;
            Vector3 pontoMundo = centro + esferaPonto;
            return new Ponto4D(pontoMundo);
        }
    }
}