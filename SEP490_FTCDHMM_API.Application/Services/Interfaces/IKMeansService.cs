using SEP490_FTCDHMM_API.Application.Dtos.KMeans;

namespace SEP490_FTCDHMM_API.Application.Services.Interfaces
{
    public interface IKMeansService
    {
        ClusterOutput Compute(List<UserVector> users, int k);
    }
}
