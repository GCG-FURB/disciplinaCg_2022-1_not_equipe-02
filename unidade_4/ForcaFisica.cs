using OpenTK;

namespace CG_N4
{
    public class ForcaFisica
    {
        public Vector3 Aceleracao;

        public ForcaFisica()
        {
            Aceleracao = Vector3.Zero;
        }

        public ForcaFisica(Vector3 aceleracao)
        {
            Aceleracao = aceleracao;
        }
    }
}