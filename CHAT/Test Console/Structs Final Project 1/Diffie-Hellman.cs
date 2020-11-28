using System.Numerics;

namespace Structs_Final_Project_1
{
    public class Diffie_Hellman
    {
        #region "Public Number" 
        public BigInteger PublicNumberGenerator(int generator, int prime, int priv)
        {
            BigInteger publicNumber = 0;

            if ((generator > 0) && (prime > 0))
            {
                if (IsPrimeNumber(prime))
                {
                    publicNumber = Exchange(priv, generator, prime);
                }
            }
            return publicNumber;
        }
        #endregion
        #region "K"
        public BigInteger KGenerator(BigInteger numberExchange, int privNumber, int prime)
        {
            BigInteger K = BigInteger.ModPow(numberExchange, privNumber, prime);
            return K;
        }
        #endregion
        #region "Function and validation"
        public bool IsPrimeNumber(int number)
        {
            int counter = 0;
            for (int j = 1; j < 10; j++)
            {
                if (number != j)
                {
                    if (number % j == 0)
                    {
                        counter++;
                    }
                }
            }
            if (counter == 1) return true; else return false;
        }
        public BigInteger Exchange(int privateNumber, int generator, int prime)
        {
            BigInteger exchangeNumber = BigInteger.ModPow(generator, privateNumber, prime);
            return exchangeNumber;
        }
        #endregion
    }
}