using SEP490_FTCDHMM_API.Application.Dtos.Common;
using SEP490_FTCDHMM_API.Application.Dtos.RecipeDtos.Recommentdation;

namespace SEP490_FTCDHMM_API.Application.Services.Interfaces.RecipeInterface
{
    public interface IRecommentdationService
    {
        Task<PagedResult<RecommendedRecipeResponse>> RecommendAsync(Guid userId, PaginationParams request);

    }
}
