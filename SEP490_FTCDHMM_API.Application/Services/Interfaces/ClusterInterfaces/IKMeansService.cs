using SEP490_FTCDHMM_API.Application.Dtos.KMeans;

namespace SEP490_FTCDHMM_API.Application.Services.Interfaces.ClusterInterfaces
{
    public interface IKMeansService
    {
        ClusterOutput Compute(List<UserVector> users, int k);
        double CalculateSilhouette(List<UserVector> users, List<UserClusterResult> assignments);
    }
}
