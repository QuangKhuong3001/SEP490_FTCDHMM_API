using SEP490_FTCDHMM_API.Application.Interfaces.Persistence;
using SEP490_FTCDHMM_API.Application.Jobs.Interfaces;
using SEP490_FTCDHMM_API.Domain.ValueObjects;

namespace SEP490_FTCDHMM_API.Application.Jobs.Implementations
{
    public class ExpireUserDietRestrictionsJob : IExpireUserDietRestrictionsJob
    {
        private readonly IUserDietRestrictionRepository _restrictionRepository;

        public ExpireUserDietRestrictionsJob(
            IUserDietRestrictionRepository restrictionRepository)
        {
            _restrictionRepository = restrictionRepository;
        }

        public async Task ExecuteAsync()
        {
            var now = DateTime.UtcNow;

            var expired = await _restrictionRepository.GetAllAsync(
                r => r.Type == RestrictionType.TemporaryAvoid &&
                     r.ExpiredAtUtc < now
            );

            if (!expired.Any())
            {
                return;
            }

            await _restrictionRepository.DeleteRangeAsync(expired);
        }
    }
}
