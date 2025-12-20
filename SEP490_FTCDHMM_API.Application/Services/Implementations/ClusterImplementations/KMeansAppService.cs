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

        public async Task<KMeansEvaluationResult> EvaluateKAsync(int minK, int maxK)
        {
            var vectors = await _userVectorBuilder.BuildAllAsync();

            if (vectors.Count < 2)
            {
                return new KMeansEvaluationResult
                {
                    BestK = 1
                };
            }


            var elbow = new Dictionary<int, double>();
            var silhouette = new Dictionary<int, double>();

            var safeMaxK = Math.Min(maxK, vectors.Count - 1);
            if (safeMaxK < minK)
                safeMaxK = minK;

            for (var k = minK; k <= safeMaxK; k++)
            {
                var result = _kMeansService.Compute(vectors, k);
                elbow[k] = result.Inertia;
                silhouette[k] = _kMeansService.CalculateSilhouette(vectors, result.Assignments);
            }

            var ordered = elbow
                .OrderBy(x => x.Value)
                .Select(x => x.Key)
                .ToList();

            var candidateKs = ordered
                .Skip(1)
                .Take(3)
                .ToList();

            var bestK = silhouette
                .Where(x => candidateKs.Any() && candidateKs.Contains(x.Key))
                .DefaultIfEmpty(silhouette.OrderByDescending(x => x.Value).First())
                .OrderByDescending(x => x.Value)
                .First()
                .Key;

            return new KMeansEvaluationResult
            {
                Elbow = elbow,
                Silhouette = silhouette,
                BestK = bestK
            };
        }
    }
}
