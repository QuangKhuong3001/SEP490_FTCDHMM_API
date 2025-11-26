using SEP490_FTCDHMM_API.Application.Dtos.KMeans;
using SEP490_FTCDHMM_API.Application.Services.Interfaces;

namespace SEP490_FTCDHMM_API.Application.Services.Implementations
{
    public class KMeansAppService : IKMeansAppService
    {
        private readonly IUserVectorBuilder _userVectorBuilder;
        private readonly IKMeansService _kMeansService;

        public KMeansAppService(
            IUserVectorBuilder userVectorBuilder,
            IKMeansService kMeansService)
        {
            _userVectorBuilder = userVectorBuilder;
            _kMeansService = kMeansService;
        }

        public async Task<ClusterOutput> ComputeAsync(int k)
        {
            var vectors = await _userVectorBuilder.BuildAllAsync();
            if (!vectors.Any())
                throw new Exception("No user vectors to cluster.");

            return _kMeansService.Compute(vectors, k);
        }
    }
}
