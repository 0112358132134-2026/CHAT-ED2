using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Structs_Final_Project
{
    public class Cesar
    {
        public Dictionary<string, string> alphabet = new Dictionary<string, string>();
        public string CesarE(string key, string message)
        {
            FillDictionary(key);

            StringBuilder encryptedMessage = new StringBuilder();
            for (int i = 0; i < message.Length; i++)
            {
                if (alphabet.ContainsKey(message[i].ToString()))
                {
                    encryptedMessage.Append(alphabet[message[i].ToString()]);
                }
                else if (alphabet.ContainsKey(message[i].ToString().ToUpper()))
                {
                    string ok = alphabet[message[i].ToString().ToUpper()];
                    encryptedMessage.Append(ok.ToLower());
                }
                else
                {
                    encryptedMessage.Append(message[i].ToString());
                }
            }
            return encryptedMessage.ToString();
        }        
        public void FillDictionary(string key)
        {

            string[] aux = { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z" };
            List<string> alphabetAux = aux.OfType<string>().ToList();

            List<string> listAux = new List<string>();
            for (int i = 0; i < key.Length; i++)
            {
                listAux.Add(key[i].ToString());
            }

            for (int i = 0; i < alphabetAux.Count; i++)
            {
                if (!listAux.Contains(alphabetAux[i]))
                {
                    listAux.Add(alphabetAux[i]);
                }
            }

            for (int i = 0; i < alphabetAux.Count; i++)
            {
                alphabet.Add(alphabetAux[i], listAux[i]);
            }
        }
    }
}