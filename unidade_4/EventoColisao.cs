namespace CG_N4
{
    public class EventoColisao
    {
        public readonly Objeto Objeto;
        public readonly ForcaFisica ForcaFisica;

        public EventoColisao(Objeto objeto, ForcaFisica forcaFisica)
        {
            Objeto = objeto;
            ForcaFisica = forcaFisica;
        }
    }
}