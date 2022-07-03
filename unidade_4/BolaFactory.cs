using CG_Biblioteca;

namespace CG_N4
{
    public abstract class BolaFactory
    {
        public static readonly float RaioBolin = (float)Utilitario.CentimetrosEmPixels(4.5d / 2.0d);
        public static readonly float RaioBocha = (float)Utilitario.CentimetrosEmPixels(11.5d / 2.0d);

        public static Esfera BuildBocha(Time time)
        {
            Esfera esfera = new Esfera(RaioBocha);
            esfera.ObjetoCor = time.CorBola;
            esfera.ForcaFisica.Massa = 1150;
            return esfera;
        }

        public static Esfera BuildBolin()
        {
            Esfera esfera = new Esfera(RaioBolin);
            esfera.ObjetoCor = new Cor(100, 100, 100);
            esfera.ForcaFisica.Massa = 375;
            return esfera;
        }
    }

    public enum TipoBola
    {
        BOLIN, BOCHA
    }
}