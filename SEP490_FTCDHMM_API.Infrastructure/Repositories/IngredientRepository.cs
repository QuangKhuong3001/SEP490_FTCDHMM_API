using Microsoft.EntityFrameworkCore;
using SEP490_FTCDHMM_API.Application.Interfaces.Persistence;
using SEP490_FTCDHMM_API.Domain.Entities;
using SEP490_FTCDHMM_API.Infrastructure.Data;

namespace SEP490_FTCDHMM_API.Infrastructure.Repositories
{
    public class IngredientRepository : EfRepository<Ingredient>, IIngredientRepository
    {
        private readonly AppDbContext _dbContext;

        public IngredientRepository(AppDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<Ingredient>> GetTop5Async(string keyword, CancellationToken ct = default)
        {
            return await _dbContext.Ingredients
                .AsNoTracking()
                .Where(i => EF.Functions.Like(i.Name.ToLower(), $"%{keyword}%"))
                .OrderByDescending(i => i.Name.ToLower().StartsWith(keyword))
                .ThenByDescending(i => i.PopularityScore)
                .ThenBy(i => i.Name.Length)
                .Take(5)
                .ToListAsync(ct);
        }
    }

}