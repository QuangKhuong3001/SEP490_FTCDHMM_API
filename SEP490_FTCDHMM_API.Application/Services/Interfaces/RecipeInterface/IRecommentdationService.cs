using SEP490_FTCDHMM_API.Application.Dtos.RecipeDtos.Recommentdation;

namespace SEP490_FTCDHMM_API.Application.Services.Interfaces.RecipeInterface
{
    public interface IRecommentdationService
    {
        Task<List<RecipeRankResponse>> RecommendAsync(Guid userId);

    }
}
