using System.Security.Cryptography;
using System.Text;

namespace SEP490_FTCDHMM_API.Shared.Utils
{
    public static class HashHelper
    {
        public static string ComputeSha256Hash(string rawData)
        {
            using var sha256 = SHA256.Create();
            var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(rawData));
            return BitConverter.ToString(bytes).Replace("-", "").ToLower();
        }
    }
}
