using SEP490_FTCDHMM_API.Application.Dtos.KMeans;
using SEP490_FTCDHMM_API.Domain.Entities;

public interface IClusterRecipeScoringSystem
{
    double CalculateClusterScore(
            Recipe recipe,
            ClusterProfile clusterProfile,
            TimeSpan currentTime);
}
