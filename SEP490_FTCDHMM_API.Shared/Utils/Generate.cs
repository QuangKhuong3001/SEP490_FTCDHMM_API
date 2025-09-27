using System.Security.Cryptography;
using System.Text;
using SEP490_FTCDHMM_API.Shared.Exceptions;

namespace SEP490_FTCDHMM_API.Shared.Utils
{
    public static class Generate
    {
        private static readonly Random _random = new Random();

        private const string _lettersUpper = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        private const string _lettersLower = "abcdefghijklmnopqrstuvwxyz";
        private const string _digits = "0123456789";
        private const string _special = "!@#$%^&*";
        private const string _all = _lettersUpper + _lettersLower + _digits + _special;

        public static string RandomString(int length)
        {
            if (length <= 0)
                throw new AppException(AppResponseCode.INVALID_ACTION);

            var result = new StringBuilder(length);
            for (int i = 0; i < length; i++)
            {
                int index = _random.Next(_all.Length);
                result.Append(_all[index]);
            }

            return result.ToString();
        }

        public static string GenerateNumericOtp(int length)
        {
            if (length < 6)
                throw new AppException(AppResponseCode.INVALID_ACTION);

            var bytes = new byte[length];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(bytes);
            var sb = new StringBuilder();
            foreach (var b in bytes)
            {
                sb.Append((b % 10).ToString());
            }
            return sb.ToString();
        }

        public static string GeneratePassword(int length)
        {
            if (length < 6)
                throw new AppException(AppResponseCode.INVALID_ACTION);

            var sb = new StringBuilder();

            sb.Append(_lettersUpper[_random.Next(_lettersUpper.Length)]);
            sb.Append(_lettersLower[_random.Next(_lettersLower.Length)]);
            sb.Append(_digits[_random.Next(_digits.Length)]);
            sb.Append(_special[_random.Next(_special.Length)]);

            for (int i = sb.Length; i < length; i++)
            {
                sb.Append(_all[_random.Next(_all.Length)]);
            }

            return new string(sb.ToString().OrderBy(c => Guid.NewGuid()).ToArray());
        }
    }
}
