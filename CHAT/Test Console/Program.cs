using System;
using System.Text;
using Structs_Final_Project;

namespace Test_Console
{
    class Program
    {
        static void Main(string[] args)
        {


            //string cadena = "Hola, cómo estás?";
            //byte[] arr = Encoding.Unicode.GetBytes(cadena);
            //string cadena2 = Encoding.Unicode.GetString(arr);

            SDES _SDES = new SDES();

            _SDES.KeysGenerator("100101");

            //byte[] asda = _SDES.SDES_Encryption(arr, "cifrado");

            //string mondo = Encoding.Unicode.GetString(asda);
            //byte[] mongo2 = Encoding.Unicode.GetBytes(mondo);

            //byte[] descifrado = _SDES.SDES_Encryption(mongo2, "descifrado");
            //string respuesta = Encoding.Unicode.GetString(descifrado);


            //Pasar texto original a bytes
            string texto = "Hola cómo estás?";
            char[] arrChar_Original = texto.ToCharArray();
            byte[] bytes_Originales = new byte[arrChar_Original.Length];

            for (int i = 0; i < arrChar_Original.Length; i++)
            {
                bytes_Originales[i] = (byte)arrChar_Original[i];
            }
            //
            //
            //
            //Cifrar bytes:
            //byte[] textoOriginal = { 23, 84, 14, 199, 200, 80, 88, 99, 40, 40, 41, 24, 42, 3};
            byte[] textoCifrado = _SDES.SDES_Encryption(bytes_Originales, "cifrado");
            //
            //
            //
            //Pasar a string el arreglo de bytes cifrado:
            char[] arrayChar_Cifrado = new char[textoCifrado.Length];
            for (int i = 0; i < textoCifrado.Length; i++)
            {
                arrayChar_Cifrado[i] = (char)textoCifrado[i];
            }
            string oka = "";
            for (int i = 0; i < arrayChar_Cifrado.Length; i++)
            {
                oka += arrayChar_Cifrado[i].ToString();
            }
            //
            //
            //
            //Pasar el string cifrado a arreglo de bytes
            char[] arrChar_cifrado = oka.ToCharArray();
            byte[] bytes_Cidrados = new byte[arrayChar_Cifrado.Length];
            for (int i = 0; i < arrChar_cifrado.Length; i++)
            {
                bytes_Cidrados[i] = (byte)arrayChar_Cifrado[i];
            }
            //
            //
            //
            //Descifrar bytes
            byte[] textoDescifrado = _SDES.SDES_Encryption(bytes_Cidrados, "descifrado");
            //
            //
            //
            //Pasar a string el arreglo de bytes descifrado:
            char[] arrayChar_Descifrado = new char[textoDescifrado.Length];
            for (int i = 0; i < textoDescifrado.Length; i++)
            {
                arrayChar_Descifrado[i] = (char)textoDescifrado[i];
            }
            string oke1 = "";
            for (int i = 0; i < arrayChar_Descifrado.Length; i++)
            {
                oke1 += arrayChar_Descifrado[i].ToString();
            }

            Console.WriteLine(oke1);
            Console.ReadLine();


            char[] charArray = oke1.ToCharArray();
            byte[] result = new byte[charArray.Length];
            for (int i = 0; i < charArray.Length; i++)
            {
                result[i] = (byte)charArray[i];
            }
            string s = Encoding.UTF8.GetString(result, 0, result.Length); 



            

            //char[] charArray = new char[textoDescifrado.Length];

            //for (int i = 0; i < textoDescifrado.Length; i++)
            //{
            //    charArray[i] = (char)textoDescifrado[i];
            //}
            //StringBuilder buil = new StringBuilder();
            //for (int i = 0; i < charArray.Length; i++)
            //{
            //    buil.Append(charArray[i].ToString());
            //}
            //string result = buil.ToString();
            //_SDES.KeysGenerator("10001000");
            //byte[] textoOriginal = { 23, 84, 14, 199, 200, 80, 88, 99, 40, 40, 41, 24, 42, 3};
            //byte[] textoCifrado = _SDES.SDES_Encryption(textoOriginal, "cifrado");
            //byte[] textoDescifrado = _SDES.SDES_Encryption(textoCifrado, "descifrado");



            Console.ReadLine();
        }
    }
}