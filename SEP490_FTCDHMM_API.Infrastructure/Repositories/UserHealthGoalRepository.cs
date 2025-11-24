using Microsoft.EntityFrameworkCore;
using SEP490_FTCDHMM_API.Application.Interfaces.Persistence;
using SEP490_FTCDHMM_API.Domain.Entities;
using SEP490_FTCDHMM_API.Infrastructure.Data;

namespace SEP490_FTCDHMM_API.Infrastructure.Repositories
{
    public class UserHealthGoalRepository : EfRepository<UserHealthGoal>, IUserHealthGoalRepository
    {
        private readonly AppDbContext _dbContext;

        public UserHealthGoalRepository(AppDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<UserHealthGoal?> GetActiveGoalByUserIdAsync(Guid userId)
        {
            return await _dbContext.UserHealthGoals
                .Include(u => u.HealthGoal)
                    .ThenInclude(hg => hg!.Targets)
                        .ThenInclude(t => t.Nutrient)
                .Include(u => u.CustomHealthGoal)
                    .ThenInclude(chg => chg!.Targets)
                        .ThenInclude(t => t.Nutrient)
                .Where(u => u.ExpiredAtUtc == null || u.ExpiredAtUtc > DateTime.UtcNow)
                .FirstOrDefaultAsync(uhg => uhg.UserId == userId);
        }
    }
}
