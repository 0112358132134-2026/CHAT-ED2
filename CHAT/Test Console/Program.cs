using System;
using System.Collections.Generic;
using Structs_Final_Project;

namespace Test_Console
{
    class Program
    {
        static void Main(string[] args)
        {           
            SDES _SDES = new SDES();
            _SDES.KeysGenerator("10001000");
            byte[] textoOriginal = { 23, 84, 14, 199, 200};
            byte[] textoCifrado = _SDES.SDES_Encryption(textoOriginal, "cifrado");
            byte[] textoDescifrado = _SDES.SDES_Encryption(textoCifrado, "descifrado");
            Console.ReadLine();
        }
    }
}
