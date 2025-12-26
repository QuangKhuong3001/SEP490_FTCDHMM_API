using Microsoft.EntityFrameworkCore;
using SEP490_FTCDHMM_API.Application.Interfaces.Persistence;
using SEP490_FTCDHMM_API.Domain.Entities;
using SEP490_FTCDHMM_API.Infrastructure.Data;

namespace SEP490_FTCDHMM_API.Infrastructure.Repositories
{
    public class MealSlotRepository : EfRepository<UserMealSlot>, IMealSlotRepository
    {
        private readonly AppDbContext _dbContext;

        public MealSlotRepository(AppDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public Task<List<UserMealSlot>> GetByUserAsync(Guid userId)
        {
            return _dbContext.Set<UserMealSlot>()
                .Where(x => x.UserId == userId)
                .OrderBy(x => x.OrderIndex)
                .ToListAsync();
        }
    }

}