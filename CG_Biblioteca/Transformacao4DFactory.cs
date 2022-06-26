using System.Collections.Generic;
using System.Threading;

namespace CG_Biblioteca
{
    public abstract class Transformacao4DFactory
    {
        private const int MaxInstances = 20;

        private static readonly Queue<Transformacao4D> Instances = new Queue<Transformacao4D>();
        private static readonly Mutex Mutex = new Mutex();

        public static Transformacao4D Get()
        {
            Mutex.WaitOne();
            try
            {
                if (Instances.Count > 0)
                {
                    Transformacao4D transformacao = Instances.Dequeue();
                    transformacao.AtribuirIdentidade();
                    return transformacao;
                }

                return new Transformacao4D();
            }
            finally
            {
                Mutex.ReleaseMutex();
            }
        }

        public static void Offer(Transformacao4D transformacao4D)
        { 
            Mutex.WaitOne();
            try
            {
                if (Instances.Count < MaxInstances)
                {
                    Instances.Enqueue(transformacao4D);
                }
            }
            finally
            {
                Mutex.ReleaseMutex();
            }
        }
    }
}