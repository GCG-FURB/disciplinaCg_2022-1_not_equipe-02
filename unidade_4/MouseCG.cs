using System;

namespace CG_N4
{
    public abstract class MouseCG
    {
        public static int X { get; private set; }
        public static int Y { get; private set; }

        public static int DeltaX { get; private set; }
        public static int DeltaY { get; private set; }

        public static void Atualizar(int x, int y)
        {
            X = x;
            Y = y;
        }

        public static void AtualizarDelta(int x, int y)
        {
            DeltaX += x;
            DeltaY += y;
        }

        public static void ResetarDelta()
        {
            DeltaX = 0;
            DeltaY = 0;
        }
    }
}