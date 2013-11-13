using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestApp
{
    class Program
    {
        static void Main(string[] args)
        {
            GUID();
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
