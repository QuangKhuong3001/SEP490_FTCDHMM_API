using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SEP490_FTCDHMM_API.Application.Dtos.Common;
using SEP490_FTCDHMM_API.Application.Dtos.RecipeDtos.Recommentdation;
using SEP490_FTCDHMM_API.Application.Interfaces.Persistence;
using SEP490_FTCDHMM_API.Application.Interfaces.SystemServices;
using SEP490_FTCDHMM_API.Application.Services.Interfaces.RecipeInterfaces;

namespace SEP490_FTCDHMM_API.Application.Services.Implementations.RecipeImplementation
{
    public class RecommendationService : IRecommentdationService
    {
        private readonly IUserRepository _userRepository;
        private readonly IRecipeRepository _recipeRepository;
        private readonly IRecipeScoringSystem _recipeScoringSystem;
        private readonly IMapper _mapper;

        public RecommendationService(
            IUserRepository userRepository,
            IRecipeRepository recipeRepository,
            IMapper mapper,
            IRecipeScoringSystem recipeScoringSystem)
        {
            _userRepository = userRepository;
            _recipeRepository = recipeRepository;
            _mapper = mapper;
            _recipeScoringSystem = recipeScoringSystem;
        }

        public async Task<PagedResult<RecipeRankResponse>> RecommendRecipesAsync(Guid userId, PaginationParams request)
        {
            var user = await _userRepository.GetByIdAsync(
                id: userId,
                include: q =>
                    q.Include(u => u.UserHealthGoals)
                        .ThenInclude(uhg => uhg!.HealthGoal)
                            .ThenInclude(hg => hg!.Targets)
                                .ThenInclude(t => t.Nutrient)
                    .Include(u => u.UserHealthGoals)
                        .ThenInclude(uhg => uhg!.CustomHealthGoal)
                            .ThenInclude(chg => chg!.Targets)
                                .ThenInclude(t => t.Nutrient)
                    .Include(u => u.Ratings)
                    .Include(u => u.DietRestrictions)
                    .Include(u => u.HealthMetrics));

            var recipes = await _recipeRepository.GetActiveRecentRecipesAsync();

            if (!recipes.Any())
            {
                return new PagedResult<RecipeRankResponse>
                {
                    Items = new List<RecipeRankResponse>(),
                    TotalCount = 0,
                    PageNumber = request.PageNumber,
                    PageSize = request.PageSize
                };
            }

            var scored = recipes
                .Select(r => (Recipe: r, Score: _recipeScoringSystem.CalculateFinalScore(user!, r)))
                .ToList();

            var ordered = scored
                .OrderByDescending(x => x.Score)
                .ThenByDescending(x => x.Recipe.UpdatedAtUtc)
                .ToList();

            var totalCount = ordered.Count;

            var pagedTuples = ordered
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToList();

            var pagedRecipes = pagedTuples.Select(x => x.Recipe).ToList();
            var mapped = _mapper.Map<List<RecipeRankResponse>>(pagedRecipes);

            for (var i = 0; i < mapped.Count; i++)
                mapped[i].Score = pagedTuples[i].Score;

            return new PagedResult<RecipeRankResponse>
            {
                Items = mapped,
                TotalCount = totalCount,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };
        }
    }
}

