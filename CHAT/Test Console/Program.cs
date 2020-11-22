using System;
using Structs_Final_Project;

namespace Test_Console
{
    class Program
    {
        static void Main(string[] args)
        {
            SDES prueba = new SDES();
            prueba.KeysGenerator("10001000");
            Console.WriteLine(prueba.k1 +"," + prueba.k2);
            Console.Read();
        }
    }
}
