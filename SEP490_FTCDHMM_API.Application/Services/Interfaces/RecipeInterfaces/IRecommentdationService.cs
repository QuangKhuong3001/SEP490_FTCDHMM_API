using SEP490_FTCDHMM_API.Application.Dtos.Common;
using SEP490_FTCDHMM_API.Application.Dtos.RecipeDtos.Recommentdation;

namespace SEP490_FTCDHMM_API.Application.Services.Interfaces.RecipeInterfaces
{
    public interface IRecommentdationService
    {
        Task<PagedResult<RecipeRankResponse>> RecommendAsync(Guid userId, PaginationParams request);

    }
}
