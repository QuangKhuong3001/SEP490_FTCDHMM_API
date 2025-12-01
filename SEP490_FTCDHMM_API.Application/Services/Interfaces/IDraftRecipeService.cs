using SEP490_FTCDHMM_API.Application.Dtos.DraftRecipeDtos;
using SEP490_FTCDHMM_API.Application.Dtos.DraftRecipeDtos.Response;

namespace SEP490_FTCDHMM_API.Application.Services.Interfaces
{
    public interface IDraftRecipeService
    {
        Task CreateDraftAsync(Guid userId, DraftRecipeRequest request);
        Task UpdateDraftAsync(Guid userId, Guid draftId, DraftRecipeRequest request);
        Task<IEnumerable<DraftRecipeResponse>> GetDraftsAsync(Guid userId);
        Task<DraftDetailsResponse> GetDraftByIdAsync(Guid userId, Guid draftId);
    }
}
