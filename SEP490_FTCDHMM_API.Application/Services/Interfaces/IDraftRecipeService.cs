using SEP490_FTCDHMM_API.Application.Dtos.DraftRecipeDtos;

namespace SEP490_FTCDHMM_API.Application.Services.Interfaces
{
    public interface IDraftRecipeService
    {
        Task CreateOrUpdateDraftAsync(Guid userId, DraftRecipeRequest request);
        Task<DraftRecipeResponse?> GetDraftAsync(Guid userId);
    }
}
