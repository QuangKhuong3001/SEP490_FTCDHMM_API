using SEP490_FTCDHMM_API.Application.Dtos.KMeans;
using SEP490_FTCDHMM_API.Application.Dtos.RecipeDtos.Recommentdation;

public interface IClusterRecipeScoringSystem
{
    double CalculateClusterScore(
            RecipeScoringSnapshot recipe,
            ClusterProfile clusterProfile,
            TimeSpan currentTime);
}
