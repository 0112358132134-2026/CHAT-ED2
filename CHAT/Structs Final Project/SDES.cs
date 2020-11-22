using System;
using System.Collections.Generic;
using System.Text;

namespace Structs_Final_Project
{
    public class SDES
    {
        public int[] P10_position = { 7, 0, 9, 6, 3, 5, 2, 4, 8, 1};
        public int[] P8_position = { 6, 4, 9, 8, 1, 3, 0, 5};
        public int[] P4_position = { 2, 0, 1, 3};
        public int[] EP_position = { 1, 0, 3, 2, 0, 2, 3, 1};
        public int[] IP_position = { 2, 7, 0, 5, 1, 6, 3, 4};
        public string k1,k2;

        public void KeysGenerator(string principalKey)
        {
            if (IsBinary(principalKey))
            {
                string newPrincipalKey = Fill(principalKey);

                string P10 = "";
                for (int i = 0; i < P10_position.Length; i++)
                {
                    P10 += newPrincipalKey[P10_position[i]].ToString();
                }

                string subDiv1 = P10.Substring(0, 5), subDiv2 = P10.Substring(5, 5);
                string LS1subDiv1 = subDiv1.Substring(1, 4) + subDiv1[0].ToString();
                string LS1subDiv2 = subDiv2.Substring(1, 4) + subDiv2[0].ToString();
                string LS1 = LS1subDiv1 + LS1subDiv2;
                string LS2 = LS1.Substring(2, 8) + LS1[0].ToString() + LS1[1].ToString();

                for (int i = 0; i < P8_position.Length; i++)
                {
                    k1 += LS1[P8_position[i]].ToString();
                }                

                for (int i = 0; i < P8_position.Length; i++)
                {
                    k2 += LS2[P8_position[i]].ToString();
                }
            }
        }

        public bool IsBinary(string principalKey)
        {
            bool result = false;

            if (principalKey.Length<=10) 
            {
                for (int i = 0; i < principalKey.Length; i++)
                {
                    if ((principalKey[i].ToString()!="1") && (principalKey[i].ToString() != "0"))
                    {
                        return false;
                    }
                }
                result = true;
            }
            return result;
        }

        public string Fill(string principalKey)
        {
            StringBuilder aux = new StringBuilder();
            aux.Append(principalKey);

            if (principalKey.Length < 10)
            {
                int remaining = 10 - principalKey.Length;
                for (int i = 0; i < remaining; i++)
                {
                    aux.Insert(0,"0");
                }
            }
            return aux.ToString();
        }
    }
}
