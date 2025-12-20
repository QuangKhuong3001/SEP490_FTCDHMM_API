using Microsoft.EntityFrameworkCore;
using SEP490_FTCDHMM_API.Application.Dtos.RecipeDtos;
using SEP490_FTCDHMM_API.Application.Dtos.RecipeDtos.Recommentdation;
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

        public async Task<IReadOnlyList<RecipeRankSource>> GetRecipesForRankingAsync(RecipeBasicFilterSpec spec)
        {
            var query = _context.Recipes
                .AsNoTracking()
                .Where(r => r.Status == RecipeStatus.Posted);

            if (spec.Difficulty != null)
                query = query.Where(r => r.Difficulty == DifficultyValue.From(spec.Difficulty));

            if (spec.Ration != null)
                query = query.Where(r => r.Ration == spec.Ration);

            if (spec.MaxCookTime != null)
                query = query.Where(r => r.CookTime <= spec.MaxCookTime);

            if (!string.IsNullOrEmpty(spec.Keyword))
                query = query.Where(r => r.NormalizedName.Contains(spec.Keyword));

            if (spec.IncludeIngredientIds.Any())
                query = query.Where(r =>
                    r.RecipeIngredients.Any(ri => spec.IncludeIngredientIds.Contains(ri.IngredientId)));

            if (spec.ExcludeIngredientIds.Any())
                query = query.Where(r =>
                    !r.RecipeIngredients.Any(ri => spec.ExcludeIngredientIds.Contains(ri.IngredientId)));

            if (spec.IncludeLabelIds.Any())
                query = query.Where(r =>
                    r.Labels.Any(l => spec.IncludeLabelIds.Contains(l.Id)));

            if (spec.ExcludeLabelIds.Any())
                query = query.Where(r =>
                    !r.Labels.Any(l => spec.ExcludeLabelIds.Contains(l.Id)));

            return await query
                .Select(r => new RecipeRankSource
                {
                    RecipeId = r.Id,
                    UpdatedAtUtc = r.UpdatedAtUtc,
                    IngredientIds = r.RecipeIngredients.Select(ri => ri.IngredientId).ToList()
                })
                .ToListAsync();
        }


        public async Task<List<RecipeScoringSnapshot>> GetRecipesForScoringAsync()
        {
            var oneYearAgo = DateTime.UtcNow.AddMonths(-12);

            return await _context.Recipes
                .AsNoTracking()
                .Where(r => r.Status == RecipeStatus.Posted &&
                            r.UpdatedAtUtc >= oneYearAgo)
                .OrderByDescending(r => r.UpdatedAtUtc)
                .Select(r => new RecipeScoringSnapshot
                {
                    Id = r.Id,
                    UpdatedAtUtc = r.UpdatedAtUtc,
                    NutritionAggregates = r.NutritionAggregates.Select(na => new NutrientSnapshot
                    {
                        NutrientId = na.NutrientId,
                        AmountPerServing = na.AmountPerServing
                    }).ToList(),
                    Calories = r.Calories,
                    Ration = r.Ration,
                    LabelIds = r.Labels.Select(l => l.Id).ToList(),
                    IngredientIds = r.RecipeIngredients.Select(ri => ri.IngredientId).ToList(),
                    IngredientCategoryIds = r.RecipeIngredients
                        .SelectMany(ri => ri.Ingredient.Categories.Select(c => c.Id))
                        .ToList()
                })
                .Take(1000)
                .ToListAsync();
        }


    }
}
