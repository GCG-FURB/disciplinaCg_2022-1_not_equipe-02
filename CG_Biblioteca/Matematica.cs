using System;
using OpenTK;

namespace CG_Biblioteca
{
    /// <summary>
    /// Classe com funções matemáticas.
    /// </summary>
    public abstract class Matematica
    {
        /// <summary>
        /// Função para calcular um ponto sobre o perímetro de um círculo informando um ângulo e raio.
        /// </summary>
        /// <param name="angulo"></param>
        /// <param name="raio"></param>
        /// <returns></returns>
        public static Ponto4D GerarPtosCirculo(double angulo, double raio)
        {
            Ponto4D pto = new Ponto4D();
            pto.X = (raio * Math.Cos(Math.PI * angulo / 180.0));
            pto.Y = (raio * Math.Sin(Math.PI * angulo / 180.0));
            pto.Z = 0;
            return (pto);
        }

        public static double GerarPtosCirculoSimétrico(double raio)
        {
            return (raio * Math.Cos(Math.PI * 45 / 180.0));
        }

        public static double InterseccaoScanLine(double yi, double y1, double y2)
        {
            return ((yi - y1) / (y2 - y1));
        }

        public static double CalculaXiScanLine(double x1, double x2, double ti)
        {
            return (x1 + (x2 - x1) * ti);
        }

        public static double Distancia(Ponto4D pontoA, Ponto4D pontoB)
        {
            return Math.Sqrt((Math.Pow(pontoA.X - pontoB.X, 2) + Math.Pow(pontoA.Y - pontoB.Y, 2) +
                              Math.Pow(pontoA.Z - pontoB.Z, 2)));
        }

        public static float Distancia(Vector3 pontoA, Vector3 pontoB)
        {
            return (float)Math.Sqrt((Math.Pow(pontoA.X - pontoB.X, 2) + Math.Pow(pontoA.Y - pontoB.Y, 2) +
                                     Math.Pow(pontoA.Z - pontoB.Z, 2)));
        }

        public static Vector3 Arredondar(Vector3 v)
        {
            float x = (float)Math.Round(v.X, 6, MidpointRounding.ToEven);
            float y = (float)Math.Round(v.Y, 6, MidpointRounding.ToEven);
            float z = (float)Math.Round(v.Z, 6, MidpointRounding.ToEven);
            return new Vector3(x, y, z);
        }
    }
}