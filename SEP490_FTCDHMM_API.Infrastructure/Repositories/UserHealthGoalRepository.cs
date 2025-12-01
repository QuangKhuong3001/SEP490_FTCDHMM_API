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
            var now = DateTime.UtcNow;

            return await _dbContext.UserHealthGoals
                .Where(u =>
                    u.UserId == userId &&
                    u.StartedAtUtc <= now &&
                    (u.ExpiredAtUtc == null || u.ExpiredAtUtc > now)
                )
                .Include(u => u.CustomHealthGoal)
                    .ThenInclude(ch => ch!.Targets)
                .Include(u => u.HealthGoal)
                    .ThenInclude(h => h!.Targets)
                .OrderByDescending(u => u.StartedAtUtc)
                .FirstOrDefaultAsync();
        }

        public async Task<List<UserHealthGoal>> GetHistoryByUserIdAsync(Guid userId)
        {
            var now = DateTime.UtcNow;

            return await _dbContext.UserHealthGoals
                .Where(u =>
                    u.UserId == userId &&
                    u.ExpiredAtUtc < now
                )
                .Include(u => u.CustomHealthGoal)
                    .ThenInclude(ch => ch!.Targets)
                .Include(u => u.HealthGoal)
                    .ThenInclude(h => h!.Targets)
                .OrderByDescending(u => u.StartedAtUtc)
                .ToListAsync();
        }
    }
}
