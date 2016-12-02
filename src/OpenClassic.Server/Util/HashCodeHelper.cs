namespace OpenClassic.Server.Util
{
    public static class HashCodeHelper
    {
        const int PrimeOne = 23;
        const int PrimeTwo = 17;

        public static int GetHashCode(short upper, short lower)
        {
            unchecked
            {
#pragma warning disable CS0675 // Bitwise-or operator used on a sign-extended operand
                return (upper << 16) | lower;
#pragma warning restore CS0675 // Bitwise-or operator used on a sign-extended operand
            }
        }

        public static int GetHashCode<T1, T2>(T1 a, T2 b)
        {
            unchecked
            {
                var hash = PrimeOne;
                hash = hash * PrimeTwo + (Equals(a, default(T1)) ? 0 : a.GetHashCode());
                hash = hash * PrimeTwo + (Equals(b, default(T2)) ? 0 : b.GetHashCode());

                return hash;
            }
        }

        public static int GetHashCode<T1, T2, T3>(T1 a, T2 b, T3 c)
        {
            unchecked
            {
                var hash = PrimeOne;
                hash = hash * PrimeTwo + (Equals(a, default(T1)) ? 0 : a.GetHashCode());
                hash = hash * PrimeTwo + (Equals(b, default(T2)) ? 0 : b.GetHashCode());
                hash = hash * PrimeTwo + (Equals(c, default(T3)) ? 0 : c.GetHashCode());

                return hash;
            }
        }

        public static int GetHashCode<T1, T2, T3, T4>(T1 a, T2 b, T3 c, T4 d)
        {
            unchecked
            {
                var hash = PrimeOne;
                hash = hash * PrimeTwo + (Equals(a, default(T1)) ? 0 : a.GetHashCode());
                hash = hash * PrimeTwo + (Equals(b, default(T2)) ? 0 : b.GetHashCode());
                hash = hash * PrimeTwo + (Equals(c, default(T3)) ? 0 : c.GetHashCode());
                hash = hash * PrimeTwo + (Equals(d, default(T4)) ? 0 : d.GetHashCode());

                return hash;
            }
        }
    }
}
