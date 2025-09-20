using System.Text;

namespace SEP490_FTCDHMM_API.Shared.Utils
{
    public class Generate
    {
        private static readonly Random _random = new Random();

        private const string _letters = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
        private const string _digits = "0123456789";
        private const string _special = "!@#$%^&*";
        private const string _all = _letters + _digits + _special;

        public static string RandomString(int length)
        {
            if (length <= 0)
                throw new ArgumentException("Length must be greater than 0");

            var result = new StringBuilder(length);
            for (int i = 0; i < length; i++)
            {
                int index = _random.Next(_all.Length);
                result.Append(_all[index]);
            }

            return result.ToString();
        }
    }
}
