using SEP490_FTCDHMM_API.Application.Dtos.KMeans;

namespace SEP490_FTCDHMM_API.Application.Services.Interfaces.ClusterInterfaces
{
    public interface IKMeansAppService
    {
        Task<ClusterOutput> ComputeAsync(int k);

    }
}
