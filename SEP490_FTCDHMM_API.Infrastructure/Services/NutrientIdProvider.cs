using System.Security.Cryptography;
using System.Text;
using SEP490_FTCDHMM_API.Domain.Interfaces;

namespace SEP490_FTCDHMM_API.Infrastructure.Services
{
    public class NutrientIdProvider : INutrientIdProvider
    {
        private static Guid Hash(string input)
        {
            using var md5 = MD5.Create();
            var hash = md5.ComputeHash(Encoding.UTF8.GetBytes(input));
            return new Guid(hash);
        }

        public Guid ProteinId => Hash("Protein");
        public Guid FatId => Hash("Fat");
        public Guid CarbohydrateId => Hash("Carbohydrate");
    }
}
