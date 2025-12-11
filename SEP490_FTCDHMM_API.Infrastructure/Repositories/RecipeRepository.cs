using Microsoft.EntityFrameworkCore;
using SEP490_FTCDHMM_API.Application.Interfaces.Persistence;
using SEP490_FTCDHMM_API.Domain.Entities;
using SEP490_FTCDHMM_API.Domain.Specifications;
using SEP490_FTCDHMM_API.Domain.ValueObjects;
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

        public async Task<IReadOnlyList<Recipe>> GetRecipesRawAsync(RecipeBasicFilterSpec spec)
        {
            var query = _context.Recipes
                .Where(r => r.Status == RecipeStatus.Posted)
                .Include(r => r.Author).ThenInclude(u => u.Avatar)
                .Include(r => r.Image)
                .Include(r => r.RecipeIngredients)
                    .ThenInclude(ri => ri.Ingredient)
                .Include(r => r.Labels)
                .Include(r => r.RecipeUserTags)
                    .ThenInclude(cs => cs.TaggedUser)
                .Include(r => r.CookingSteps)
                    .ThenInclude(cs => cs.CookingStepImages)
                        .ThenInclude(cs => cs.Image)
                .AsQueryable();

            if (spec.Difficulty != null)
                query = query.Where(r => r.Difficulty == DifficultyValue.From(spec.Difficulty));

            if (spec.Ration != null)
                query = query.Where(r => r.Ration == spec.Ration);

            if (spec.MaxCookTime != null)
                query = query.Where(r => r.CookTime <= spec.MaxCookTime);

            if (spec.IncludeIngredientIds.Any())
                query = query.Where(r =>
                    r.RecipeIngredients.Any(ri => spec.IncludeIngredientIds.Contains(ri.IngredientId)));

            if (spec.ExcludeIngredientIds.Any())
                query = query.Where(r =>
                    !r.RecipeIngredients.Any(ri => spec.ExcludeIngredientIds.Contains(ri.IngredientId)));

            if (spec.IncludeLabelIds.Any())
                query = query.Where(r =>
                    r.Labels.Any(lbl => spec.IncludeLabelIds.Contains(lbl.Id)));

            if (spec.ExcludeLabelIds.Any())
                query = query.Where(r =>
                    !r.Labels.Any(lbl => spec.ExcludeLabelIds.Contains(lbl.Id)));

            if (!string.IsNullOrEmpty(spec.Keyword))
                query = query.Where(r => r.NormalizedName.Contains(spec.Keyword));

            return await query.ToListAsync();
        }

        public async Task<List<Recipe>> GetActiveRecentRecipesAsync()
        {
            var oneYearAgo = DateTime.UtcNow.AddMonths(-12);

            var recipes = await _context.Recipes
                .Where(r => r.Status == RecipeStatus.Posted &&
                            r.UpdatedAtUtc >= oneYearAgo)
                .Include(r => r.RecipeIngredients)
                    .ThenInclude(r => r.Ingredient)
                        .ThenInclude(r => r.IngredientNutrients)
                .Include(r => r.Author)
                    .ThenInclude(a => a.Avatar)
                .Include(r => r.Image)
                .Include(r => r.Labels)
                .Include(r => r.NutritionAggregates)
                    .ThenInclude(na => na.Nutrient)
                .ToListAsync();

            var sorted = recipes.OrderByDescending(r => r.UpdatedAtUtc);

            return sorted.Take(500).ToList();
        }

    }
}
