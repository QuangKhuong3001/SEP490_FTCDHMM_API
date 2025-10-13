using SEP490_FTCDHMM_API.Application.Dtos.GoogleAuthDtos;
using SEP490_FTCDHMM_API.Domain.Entities;

namespace SEP490_FTCDHMM_API.Application.Interfaces.ExternalServices
{
    public interface IGoogleProvisioningService
    {
        Task<AppUser> FindOrProvisionFromGoogleAsync(GoogleProvisionRequest request);
    }
}
