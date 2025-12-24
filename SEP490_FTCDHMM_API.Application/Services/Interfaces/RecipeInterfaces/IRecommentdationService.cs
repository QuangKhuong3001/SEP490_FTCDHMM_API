using SEP490_FTCDHMM_API.Application.Dtos.Common;
using SEP490_FTCDHMM_API.Application.Dtos.MealDtos;
using SEP490_FTCDHMM_API.Application.Dtos.RecipeDtos.Recommentdation;

namespace SEP490_FTCDHMM_API.Application.Services.Interfaces.RecipeInterfaces
{
    public interface IRecommentdationService
    {
        Task<PagedResult<RecipeRankResponse>> RecommendRecipesAsync(Guid userId, PaginationParams request);
        Task<PagedResult<RecipeRankResponse>> ComputedRecommendRecipesAsync(Guid userId);
        Task<MealAnalyzeResponse> AnalyzeMealAsync(Guid userId, MealAnalyzeRequest request);
    }
}
