using Microsoft.EntityFrameworkCore;
using SEP490_FTCDHMM_API.Application.Interfaces;
using SEP490_FTCDHMM_API.Domain.Entities;
using SEP490_FTCDHMM_API.Domain.ValueObjects;
using SEP490_FTCDHMM_API.Infrastructure.Data;

namespace SEP490_FTCDHMM_API.Infrastructure.Repositories
{
    public class OtpRepository : EfRepository<EmailOtp>, IOtpRepository
    {
        private readonly AppDBContext _dbContext;

        public OtpRepository(AppDBContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<EmailOtp?> GetLatestAsync(string userId, OtpPurpose purpose)
        {
            return await _dbContext.EmailOtps
                .Where(o => o.UserId == userId && o.Purpose == purpose)
                .OrderByDescending(o => o.CreatedAtUtc)
                .FirstOrDefaultAsync();
        }
    }

}