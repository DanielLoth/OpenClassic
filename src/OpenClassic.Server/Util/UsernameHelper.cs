namespace OpenClassic.Server.Util
{
    public static class UsernameHelper
    {
        public static string HashToUsername(long usernameHash)
        {
            if (usernameHash < 0L)
                return "invalid_name";

            string s = string.Empty;
            while (usernameHash != 0L)
            {
                int i = (int)(usernameHash % 37L);
                usernameHash /= 37L;
                if (i == 0)
                    s = " " + s;
                else if (i < 27)
                {
                    if (usernameHash % 37L == 0L)
                        s = (char)((i + 65) - 1) + s;
                    else
                        s = (char)((i + 97) - 1) + s;
                }
                else
                {
                    s = (char)((i + 48) - 27) + s;
                }
            }
            return s;
        }

        public static long UsernameToHash(string username)
        {
            username = username.ToLower();
            var s1 = string.Empty;
            var usernameCharArray = username.ToCharArray();

            for (int i = 0; i < username.Length; i++)
            {
                char c = usernameCharArray[i];
                if (c >= 'a' && c <= 'z')
                    s1 = s1 + c;
                else if (c >= '0' && c <= '9')
                    s1 = s1 + c;
                else
                    s1 = s1 + ' ';
            }

            s1 = s1.Trim();

            if (s1.Length > 12)
            {
                s1 = s1.Substring(0, 12);
            }

            long l = 0L;
            var s1CharArray = s1.ToCharArray();
            for (int j = 0; j < s1.Length; j++)
            {
                char c1 = s1CharArray[j];
                l *= 37L;
                if (c1 >= 'a' && c1 <= 'z')
                    l += (1 + c1) - 97;
                else if (c1 >= '0' && c1 <= '9')
                    l += (27 + c1) - 48;
            }

            return l;
        }
    }
}
