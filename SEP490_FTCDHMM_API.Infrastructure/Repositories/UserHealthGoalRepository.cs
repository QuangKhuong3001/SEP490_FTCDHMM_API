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
                .Where(u => u.ExpiredAtUtc == null || u.ExpiredAtUtc > DateTime.UtcNow)
                .Include(u => u.CustomHealthGoal)
                    .ThenInclude(ch => ch!.Targets)
                .Include(u => u.HealthGoal)
                    .ThenInclude(h => h!.Targets)
                .FirstOrDefaultAsync(uhg => uhg.UserId == userId);
        }
    }
}
