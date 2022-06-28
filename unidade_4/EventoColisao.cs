using OpenTK;

namespace CG_N4
{
    public class EventoColisao
    {
        public readonly Objeto Objeto;
        public readonly Vector3 Aceleracao;

        public EventoColisao(Objeto objeto, Vector3 aceleracao)
        {
            Objeto = objeto;
            Aceleracao = aceleracao;
        }
    }
}