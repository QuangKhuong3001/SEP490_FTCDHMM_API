using Microsoft.EntityFrameworkCore;
using SEP490_FTCDHMM_API.Application.Interfaces.Persistence;
using SEP490_FTCDHMM_API.Domain.Entities;
using SEP490_FTCDHMM_API.Infrastructure.Data;

namespace SEP490_FTCDHMM_API.Infrastructure.Repositories
{
    public class RecipeRepository : EfRepository<Recipe>, IRecipeRepository
    {
        private readonly AppDbContext _context;
        public RecipeRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<List<Recipe>> GetActiveRecentRecipesAsync()
        {
            var oneYearAgo = DateTime.UtcNow.AddMonths(-12);

            return await _context.Recipes
                .Where(r => !r.IsDeleted &&
                            r.CreatedAtUtc >= oneYearAgo)
                .Include(r => r.Author)
                    .ThenInclude(u => u.Avatar)
                .Include(r => r.Image)
                .Include(r => r.Labels)
                .Include(r => r.RecipeIngredients)
                    .ThenInclude(r => r.Ingredient)
                .Include(r => r.NutritionAggregates)
                    .ThenInclude(na => na.Nutrient)
                .Include(r => r.Parent)
                .ToListAsync();
        }

    }
}
