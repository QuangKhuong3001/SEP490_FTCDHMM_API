using SEP490_FTCDHMM_API.Application.Dtos.RecipeDtos;

namespace SEP490_FTCDHMM_API.Application.Services.Interfaces.RecipeInterface
{
    public interface IRecipeManagementService
    {
        Task LockAsync(Guid userId, Guid recipeId, RecipeManagementReasonRequest request);
        Task ApproveAsync(Guid userId, Guid recipeId);
        Task RejectAsync(Guid userId, Guid recipeId, RecipeManagementReasonRequest request);
        Task DeleteAsync(Guid userId, Guid recipeId, RecipeManagementReasonRequest request);
    }
}
