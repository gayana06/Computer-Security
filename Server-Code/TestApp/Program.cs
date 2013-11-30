using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.IO;
using System.Security.Cryptography;

namespace TestApp
{
    class Program
    {
        private static Timer ticker;
        static void Main(string[] args)
        {
            //GUID();
            /*
            for (int i = 0; i < 50; i++)
            {
                StartTimer();
                Thread.Sleep(3000);
                Resetimer();
                Thread.Sleep(10000);
            }

                Console.WriteLine("Press the Enter key to end the program.");
            Console.ReadLine();
             * */
            try
            {
              //  Program p = new Program();
              //  p.testDec();
              //  p.testEnc();
               // var Files = Directory.EnumerateFiles(@"E:\EMDC_IST\SEMESTER_1\SIRS-Computer_Security\PROJECT\TEST\new", "*.aes", SearchOption.AllDirectories);
                //CheckOpen.check();

                string text = "Gayana@1234";
                Console.WriteLine("SHA1="+GetSHA1Hash(text));
                Console.WriteLine("SHA2=" + GetSHA2Hash(text));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message+"----"+ex.StackTrace);
            }
        }

        public static string GetSHA1Hash(string text)
        {
            byte[] buffer = Encoding.ASCII.GetBytes(text);
            SHA1CryptoServiceProvider cryptoTransformSHA1 = new SHA1CryptoServiceProvider();
            return BitConverter.ToString(cryptoTransformSHA1.ComputeHash(buffer)).Replace("-", "").ToLower();
        }

        public static string GetSHA2Hash(string text)
        {
            byte[] buffer = Encoding.ASCII.GetBytes(text);
            SHA256CryptoServiceProvider cryptoTransformSHA2 = new SHA256CryptoServiceProvider();
            return BitConverter.ToString(cryptoTransformSHA2.ComputeHash(buffer)).Replace("-", "").ToLower();
        }

        public void testEnc()
        {
            sirsCryptClass s = new sirsCryptClass();
            String dir = @"E:\EMDC_IST\SEMESTER_1\SIRS-Computer_Security\PROJECT\TEST\GAYANA";

            s.EncryptDirectory(dir, "cf6fc152d377f880ec593d011d07d07970d6a3b6");
            Console.WriteLine("Done");
        }
        public void testDec()
        {
            sirsCryptClass s = new sirsCryptClass();
            String dir = @"E:\EMDC_IST\SEMESTER_1\SIRS-Computer_Security\PROJECT\TEST\GAYANA";

            s.DecryptDirectory(dir, "cf6fc152d377f880ec593d011d07d07970d6a3b6");
            Console.WriteLine("Done");
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
