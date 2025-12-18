using SEP490_FTCDHMM_API.Application.Dtos.KMeans;
using SEP490_FTCDHMM_API.Application.Services.Interfaces.ClusterInterfaces;

namespace SEP490_FTCDHMM_API.Application.Services.Implementations.ClusterImplementations
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

            return _kMeansService.Compute(vectors, k);
        }
    }
}
