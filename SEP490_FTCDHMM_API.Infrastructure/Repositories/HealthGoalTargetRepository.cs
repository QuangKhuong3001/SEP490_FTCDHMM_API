using SEP490_FTCDHMM_API.Application.Interfaces.Persistence;
using SEP490_FTCDHMM_API.Domain.Entities;
using SEP490_FTCDHMM_API.Infrastructure.Data;

namespace SEP490_FTCDHMM_API.Infrastructure.Repositories
{
    public class HealthGoalTargetRepository : EfRepository<HealthGoalTarget>, IHealthGoalTargetRepository
    {
        private readonly AppDbContext _dbContext;

        public HealthGoalTargetRepository(AppDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }
    }
}
