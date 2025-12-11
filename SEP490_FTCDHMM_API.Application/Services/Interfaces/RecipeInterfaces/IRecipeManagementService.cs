using SEP490_FTCDHMM_API.Application.Dtos.RecipeDtos;

namespace SEP490_FTCDHMM_API.Application.Services.Interfaces.RecipeInterfaces
{
    public interface IRecipeManagementService
    {
        Task LockRecipeAsync(Guid userId, Guid recipeId, RecipeManagementReasonRequest request);
        Task ApproveRecipeAsync(Guid userId, Guid recipeId);
        Task RejectRecipeAsync(Guid userId, Guid recipeId, RecipeManagementReasonRequest request);
        Task DeleteRecipeByManageAsync(Guid userId, Guid recipeId, RecipeManagementReasonRequest request);
    }
}
