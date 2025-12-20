using Microsoft.Extensions.Options;
using SEP490_FTCDHMM_API.Application.Configurations;
using SEP490_FTCDHMM_API.Application.Dtos.KMeans;
using SEP490_FTCDHMM_API.Application.Interfaces.SystemServices;
using SEP490_FTCDHMM_API.Application.Jobs.Interfaces.PreComputedInterfaces;
using SEP490_FTCDHMM_API.Application.Services.Interfaces.ClusterInterfaces;

public class ClusterAssignmentJob : IClusterAssignmentJob
{
    private readonly IKMeansAppService _kMeansAppService;
    private readonly ICacheService _cache;
    private readonly KMeansSettings _settings;

    public ClusterAssignmentJob(
        IKMeansAppService kMeansAppService,
        ICacheService cache,
        IOptions<KMeansSettings> settings)
    {
        _kMeansAppService = kMeansAppService;
        _cache = cache;
        _settings = settings.Value;
    }

    public async Task ExecuteAsync()
    {
        await _cache.RemoveByPrefixAsync("cluster");
        await _cache.DeleteKeyAsync("cluster:profiles");

        var evaluation = await _kMeansAppService.EvaluateKAsync(
            _settings.MinK,
            _settings.MaxK);

        var k = evaluation.BestK;

        var result = await _kMeansAppService.ComputeAsync(k);

        foreach (var a in result.Assignments)
        {
            await _cache.SetAsync(
                $"cluster:user:{a.UserId}",
                a.ClusterId,
                TimeSpan.FromDays(7));
        }

        foreach (var c in result.Centroids)
        {
            var profile = new ClusterProfile
            {
                ClusterId = c.Key,
                Tdee = c.Value[0],
                CarbPct = c.Value[1],
                ProteinPct = c.Value[2],
                FatPct = c.Value[3]
            };

            await _cache.HashSetAsync(
                "cluster:profiles",
                profile.ClusterId.ToString(),
                profile,
                TimeSpan.FromDays(7));
        }
    }
}