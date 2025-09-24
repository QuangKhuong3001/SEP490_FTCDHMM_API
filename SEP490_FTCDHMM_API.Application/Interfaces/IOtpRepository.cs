using SEP490_FTCDHMM_API.Domain.Entities;
using SEP490_FTCDHMM_API.Domain.ValueObjects;

namespace SEP490_FTCDHMM_API.Application.Interfaces
{
    public interface IOtpRepository : IRepository<EmailOtp>
    {
        Task<EmailOtp?> GetLatestAsync(string userId, OtpPurpose purpose);
    }
}
