using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SEP490_FTCDHMM_API.Application.Configurations;
using SEP490_FTCDHMM_API.Application.Dtos.KMeans;
using SEP490_FTCDHMM_API.Application.Interfaces.Persistence;
using SEP490_FTCDHMM_API.Application.Interfaces.SystemServices;
using SEP490_FTCDHMM_API.Application.Jobs.Interfaces.PreComputedInterfaces;
using SEP490_FTCDHMM_API.Application.Services.Interfaces.ClusterInterfaces;

public class ClusterAssignmentJob : IClusterAssignmentJob
{
    private readonly IKMeansAppService _kMeansAppService;
    private readonly ICacheService _cache;
    private readonly IUserRepository _userRepository;
    private readonly KMeansSettings _settings;
    private readonly ILogger<ClusterAssignmentJob> _logger;

    public ClusterAssignmentJob(
        IKMeansAppService kMeansAppService,
        ICacheService cache,
        IUserRepository userRepository,
        ILogger<ClusterAssignmentJob> logger,
        IOptions<KMeansSettings> settings)
    {
        _kMeansAppService = kMeansAppService;
        _cache = cache;
        _logger = logger;
        _userRepository = userRepository;
        _settings = settings.Value;
    }

    public async Task ExecuteAsync()
    {
        await _cache.RemoveByPrefixAsync("cluster");
        await _cache.DeleteKeyAsync("cluster:profiles");

        var userCount = await _userRepository.CountAsync();

        var k = CalculateClusterCount(userCount);

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

            _logger.LogWarning("✅ Added cluster profile {ClusterId}", profile.ClusterId);
        }

        _logger.LogWarning("✅ Added cluster profile");

    }

    private int CalculateClusterCount(int userCount)
    {
        var estimated = userCount / _settings.UserPerClusterRatio;

        if (estimated < 1)
            return 1;

        return estimated;
    }
}