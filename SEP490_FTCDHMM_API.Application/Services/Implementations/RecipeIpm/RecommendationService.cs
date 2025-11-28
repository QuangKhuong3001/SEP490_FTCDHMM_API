using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SEP490_FTCDHMM_API.Application.Dtos.Common;
using SEP490_FTCDHMM_API.Application.Dtos.RecipeDtos.Recommentdation;
using SEP490_FTCDHMM_API.Application.Interfaces.Persistence;
using SEP490_FTCDHMM_API.Application.Interfaces.SystemServices;
using SEP490_FTCDHMM_API.Application.Services.Interfaces.RecipeInterface;
using SEP490_FTCDHMM_API.Domain.Entities;
using SEP490_FTCDHMM_API.Shared.Exceptions;

namespace SEP490_FTCDHMM_API.Application.Services.Implementations.RecipeIpm
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
            IRecipeScoringSystem recipeScoringSystem,
            IMapper mapper)
        {
            _userRepository = userRepository;
            _recipeRepository = recipeRepository;
            _recipeScoringSystem = recipeScoringSystem;
            _mapper = mapper;
        }

        public async Task<PagedResult<RecommendedRecipeResponse>> RecommendAsync(Guid userId, PaginationParams request)
        {
            var user = await _userRepository.GetByIdAsync(
                id: userId,
                include: q =>
                    q.Include(u => u.UserHealthGoal)
                        .ThenInclude(uhg => uhg!.HealthGoal)
                            .ThenInclude(hg => hg!.Targets)
                                .ThenInclude(t => t.Nutrient)
                    .Include(u => u.UserHealthGoal)
                        .ThenInclude(uhg => uhg!.CustomHealthGoal)
                            .ThenInclude(chg => chg!.Targets)
                                .ThenInclude(t => t.Nutrient)
                    .Include(u => u.HealthMetrics));


            if (user == null)
            {
                throw new AppException(AppResponseCode.INVALID_ACCOUNT_INFORMATION);
            }

            var recipes = await _recipeRepository.GetActiveRecentRecipesWithDetailsAsync();

            var ranked = new List<(Recipe Recipe, double Score)>();

            foreach (var r in recipes)
            {
                var final = _recipeScoringSystem.CalculateFinalScore(user, r);
                ranked.Add((r, final));
            }

            var sorted = ranked.OrderByDescending(x => x.Score);

            var totalCount = sorted.Count();

            var pagedItems = sorted
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToList();

            var result = pagedItems.Select(item =>
            {
                var response = _mapper.Map<RecommendedRecipeResponse>(item.Recipe);
                response.Score = item.Score;
                return response;
            }).ToList();

            return new PagedResult<RecommendedRecipeResponse>
            {
                Items = result,
                TotalCount = totalCount,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };
        }
    }
}
