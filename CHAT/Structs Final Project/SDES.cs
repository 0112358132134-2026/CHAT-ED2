using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;

namespace Structs_Final_Project
{
    public class SDES
    {
        #region "Global Variables"
        public int[] P10_position = { 7, 0, 9, 6, 3, 5, 2, 4, 8, 1};
        public int[] P8_position = { 6, 4, 9, 8, 1, 3, 0, 5};
        public int[] P4_position = { 2, 0, 1, 3};
        public int[] EP_position = { 1, 0, 3, 2, 0, 2, 3, 1};
        public int[] IP_position = { 2, 7, 0, 5, 1, 6, 3, 4};
        public string k1,k2;
        public string [,] S0 = { { "01", "00", "11", "10" }, { "11", "10", "01", "00" }, { "00", "10", "01", "11" }, { "11", "01", "11", "10" } };
        public string [,] S1 = { { "00", "01", "10", "11" }, { "10", "00", "01", "11" }, { "11", "00", "01", "00" }, { "10", "01", "00", "11" } };
        #endregion
        #region "Keys"
        public void KeysGenerator(string principalKey)
        {
            if (IsBinary(principalKey))
            {
                string newPrincipalKey = Fill(principalKey, 10);

                string P10 = "";
                for (int i = 0; i < P10_position.Length; i++)
                {
                    P10 += newPrincipalKey[P10_position[i]];
                }

                string subDiv1 = P10.Substring(0, 5), subDiv2 = P10.Substring(5, 5);
                string LS1subDiv1 = subDiv1.Substring(1, 4) + subDiv1[0].ToString();
                string LS1subDiv2 = subDiv2.Substring(1, 4) + subDiv2[0].ToString();
                string LS1 = LS1subDiv1 + LS1subDiv2;
                string LS2 = LS1.Substring(2, 8) + LS1[0].ToString() + LS1[1].ToString();

                for (int i = 0; i < P8_position.Length; i++)
                {
                    k1 += LS1[P8_position[i]];
                }                

                for (int i = 0; i < P8_position.Length; i++)
                {
                    k2 += LS2[P8_position[i]];
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
        public string Fill(string principalKey, int length)
        {
            StringBuilder aux = new StringBuilder();
            aux.Append(principalKey);

            if (principalKey.Length < length)
            {
                int remaining = length - principalKey.Length;
                for (int i = 0; i < remaining; i++)
                {
                    aux.Insert(0,"0");
                }
            }
            return aux.ToString();
        }
        #endregion
        #region "SDES"
        public byte[] SDES_Encryption(byte[] originalText, string type)
        {
            byte[] result = new byte [originalText.Length];

            for (int i = 0; i < originalText.Length; i++)
            {
                #region "Start part one"
                string actualByte = Fill(ConvertDecimalToBinary(originalText[i]), 8);

                string IP = "";
                for (int j = 0; j < IP_position.Length; j++)
                {
                    IP += actualByte[IP_position[j]];
                }

                string lastFour = IP.Substring(4, 4);
                string EP = "";
                for (int j = 0; j  < EP_position.Length; j ++)
                {
                    EP += lastFour[EP_position[j]];
                }

                string _XOR = XOR1(EP, type);
                int row1 = ConvertBinaryToDecimal(_XOR.Substring(0,1) + _XOR.Substring(3,1));
                int column1 = ConvertBinaryToDecimal(_XOR.Substring(1, 1) + _XOR.Substring(2, 1));
                int row2 = ConvertBinaryToDecimal(_XOR.Substring(4, 1) + _XOR.Substring(7, 1));
                int column2 = ConvertBinaryToDecimal(_XOR.Substring(5, 1) + _XOR.Substring(6, 1));
                string newChain = S0[row1, column1] + S1[row2, column2];

                string P4 = "";
                for (int j = 0; j < P4_position.Length; j++)
                {
                    P4 += newChain[P4_position[j]];
                }
                string firstFour = IP.Substring(0, 4);
                string XOR_IP_P4 = "";
                for (int j = 0; j < 4; j++)
                {
                    if (firstFour[j].ToString() == P4[j].ToString())
                    {
                        XOR_IP_P4 += "0";
                    }
                    else
                    {
                        XOR_IP_P4 += "1";
                    }
                }
                #endregion
                #region "Start second part"
                string swap = lastFour + XOR_IP_P4;

                lastFour = swap.Substring(4, 4);
                EP = "";
                for (int j = 0; j < EP_position.Length; j++)
                {
                    EP += lastFour[EP_position[j]];
                }

                _XOR = XOR2(EP, type);
                row1 = ConvertBinaryToDecimal(_XOR.Substring(0, 1) + _XOR.Substring(3, 1));
                column1 = ConvertBinaryToDecimal(_XOR.Substring(1, 1) + _XOR.Substring(2, 1));
                row2 = ConvertBinaryToDecimal(_XOR.Substring(4, 1) + _XOR.Substring(7, 1));
                column2 = ConvertBinaryToDecimal(_XOR.Substring(5, 1) + _XOR.Substring(6, 1));
                newChain = S0[row1, column1] + S1[row2, column2];

                P4 = "";
                for (int j = 0; j < P4_position.Length; j++)
                {
                    P4 += newChain[P4_position[j]];
                }
                firstFour = swap.Substring(0, 4);
                XOR_IP_P4 = "";
                for (int j = 0; j < 4; j++)
                {
                    if (firstFour[j].ToString() == P4[j].ToString())
                    {
                        XOR_IP_P4 += "0";
                    }
                    else
                    {
                        XOR_IP_P4 += "1";
                    }
                }
                string final = XOR_IP_P4 + lastFour;

                string IP_Reverse = "";
                int counter = 0;
                List<int> lst = IP_position.OfType<int>().ToList();
                for (int j = 0; j < IP_position.Length; j++)
                {
                    int pos = lst.IndexOf(counter);
                    IP_Reverse += final[pos];
                    counter++;
                }

                result[i] = (byte)ConvertBinaryToDecimal(IP_Reverse);
                #endregion
            }
            return result;
        }
        public string XOR1(string EP, string type)
        {
            string k = "";

            if (type == "cifrado")
            {
                k = k1;
            }
            else if (type == "descifrado")
            {
                k = k2;
            }

            string result = "";
            for (int j = 0; j < 8; j++)
            {
                if (EP[j].ToString() == k[j].ToString())
                {
                    result += "0";
                }
                else
                {
                    result += "1";
                }
            }
            return result;
        }
        public string XOR2(string EP, string type)
        {
            string k = "";

            if (type == "cifrado")
            {
                k = k2;
            }
            else if (type == "descifrado")
            {
                k = k1;
            }

            string result = "";
            for (int j = 0; j < 8; j++)
            {
                if (EP[j].ToString() == k[j].ToString())
                {
                    result += "0";
                }
                else
                {
                    result += "1";
                }
            }
            return result;
        }
        #endregion
        #region "Auxiliaries"
        public int ConvertBinaryToDecimal(string binary)
        {
            StringBuilder auxiliar = new StringBuilder();
            auxiliar.Append(binary);

            int exponent = auxiliar.Length - 1;
            int decimalNumber = 0;

            for (int i = 0; i < auxiliar.Length; i++)
            {
                if (int.Parse(auxiliar.ToString(i, 1)) == 1)
                {
                    decimalNumber += int.Parse(System.Math.Pow(2, double.Parse(exponent.ToString())).ToString());
                }
                exponent--;
            }
            return decimalNumber;
        }
        public string ConvertDecimalToBinary(int number)
        {
            string result = "";
            while (number > 0)
            {
                if (number % 2 == 0)
                {
                    result = "0" + result;
                }
                else
                {
                    result = "1" + result;
                }
                number = (int)(number / 2);
            }
            return result;
        }
        public string ToNBase(BigInteger a, int n)
        {
            StringBuilder sb = new StringBuilder();
            while (a > 0)
            {
                sb.Insert(0, a % n);
                a /= n;
            }
            return sb.ToString();
        }
        #endregion
    }
}