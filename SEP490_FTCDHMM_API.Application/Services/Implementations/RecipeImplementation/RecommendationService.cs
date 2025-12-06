using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SEP490_FTCDHMM_API.Application.Dtos.Common;
using SEP490_FTCDHMM_API.Application.Dtos.RecipeDtos.Recommentdation;
using SEP490_FTCDHMM_API.Application.Interfaces.Persistence;
using SEP490_FTCDHMM_API.Application.Interfaces.SystemServices;
using SEP490_FTCDHMM_API.Application.Services.Interfaces.RecipeInterfaces;
using SEP490_FTCDHMM_API.Shared.Exceptions;

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

        public async Task<PagedResult<RecipeRankResponse>> RecommendAsync(Guid userId, PaginationParams request)
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
                    .Include(u => u.HealthMetrics));


            if (user == null)
            {
                throw new AppException(AppResponseCode.INVALID_ACCOUNT_INFORMATION);
            }

            var recipes = await _recipeRepository.GetActiveRecentRecipesAsync();

            var ranked = new List<RecipeRankResponse>();

            foreach (var r in recipes)
            {
                var final = _recipeScoringSystem.CalculateFinalScore(user, r);

                var result = _mapper.Map<RecipeRankResponse>(r);
                result.Score = final;
                ranked.Add(result);
            }

            var sorted = ranked.OrderByDescending(x => x.Score);

            var totalCount = sorted.Count();

            var pagedItems = sorted
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToList();

            return new PagedResult<RecipeRankResponse>
            {
                Items = pagedItems,
                TotalCount = totalCount,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };
        }
    }
}

