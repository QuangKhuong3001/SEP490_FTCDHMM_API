using Microsoft.EntityFrameworkCore;
using SEP490_FTCDHMM_API.Application.Interfaces.Persistence;
using SEP490_FTCDHMM_API.Domain.Entities;
using SEP490_FTCDHMM_API.Infrastructure.Data;

namespace SEP490_FTCDHMM_API.Infrastructure.Repositories
{
    public class HealthGoalRepository : EfRepository<HealthGoal>, IHealthGoalRepository
    {
        private readonly AppDbContext _dbContext;

        public HealthGoalRepository(AppDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<HealthGoal>> GetAllWithTargetsAsync()
        {
            return await _dbContext.HealthGoals
                .Include(chg => chg.Targets)
                    .ThenInclude(t => t.Nutrient)
                .ToListAsync();
        }
    }
}
