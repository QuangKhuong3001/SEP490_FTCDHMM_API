using Microsoft.EntityFrameworkCore;
using SEP490_FTCDHMM_API.Application.Interfaces.Persistence;
using SEP490_FTCDHMM_API.Domain.Entities;
using SEP490_FTCDHMM_API.Infrastructure.Data;

namespace SEP490_FTCDHMM_API.Infrastructure.Repositories
{
    public class UserBehaviorRepository : EfRepository<UserLabelStat>, IUserBehaviorRepository
    {
        private readonly AppDbContext _dbContext;

        public UserBehaviorRepository(AppDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<UserLabelStat?> GetAsync(Guid userId, Guid labelId)
        {
            return await _dbContext.UserLabelStats
                .FirstOrDefaultAsync(x => x.UserId == userId && x.LabelId == labelId);
        }

        public async Task<IEnumerable<UserLabelStat>> GetAllForUserAsync(Guid userId)
        {
            return await _dbContext.UserLabelStats
                .Where(x => x.UserId == userId)
                .ToListAsync();
        }
    }
}