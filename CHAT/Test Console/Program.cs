﻿using System;
using Structs_Final_Project;

namespace Test_Console
{
    class Program
    {
        static void Main(string[] args)
        {           
            SDES _SDES = new SDES();
            _SDES.KeysGenerator("10001000");
            byte[] textoOriginal = { 23, 84, 14, 199, 200, 80, 88, 99, 40, 40, 41, 24, 42, 3};
            byte[] textoCifrado = _SDES.SDES_Encryption(textoOriginal, "cifrado");
            byte[] textoDescifrado = _SDES.SDES_Encryption(textoCifrado, "descifrado");

            DateTime dateNow = DateTime.Now;
            //dateNow.ToLocalTime();
            Console.WriteLine("The date and time are {0} UTC.",
                               TimeZoneInfo.ConvertTimeBySystemTimeZoneId(dateNow, "Central Standard Time"));

            Console.ReadLine();
        }
    }
}