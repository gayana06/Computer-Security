using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace TestApp
{
    class Program
    {
        private static Timer ticker;
        static void Main(string[] args)
        {
            //GUID();
            for (int i = 0; i < 50; i++)
            {
                StartTimer();
                Thread.Sleep(3000);
                Resetimer();
                Thread.Sleep(10000);
            }

                Console.WriteLine("Press the Enter key to end the program.");
            Console.ReadLine();
        }

        public static void StartTimer()
        {
            ticker = new Timer(TimerMethod, null, 0, 1000);
        }

        public static void Resetimer()
        {
            ticker.Dispose();
        }

        static int index = 0;
        public static void TimerMethod(object state)
        {
            Console.Write(index+" ");
        }


        public static void GUID()
        {
            for (int i = 0; i < 20; i++)
            {
                Guid guid = Guid.NewGuid();
                Console.WriteLine(guid.ToString());
            }

        }
    }
}
