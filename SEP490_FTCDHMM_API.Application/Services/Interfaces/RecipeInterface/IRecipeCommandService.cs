using SEP490_FTCDHMM_API.Application.Dtos.RecipeDtos;

namespace SEP490_FTCDHMM_API.Application.Services.Interfaces.RecipeInterface
{
    public interface IRecipeCommandService
    {
        Task CreateRecipeAsync(Guid userId, CreateRecipeRequest request);
        Task UpdateRecipeAsync(Guid userId, Guid recipeId, UpdateRecipeRequest request);
        Task DeleteRecipeAsync(Guid userId, Guid recipeId);

        Task SaveRecipeAsync(Guid userId, Guid recipeId);
        Task UnsaveRecipeAsync(Guid userId, Guid recipeId);
        Task CopyRecipe(Guid userId, Guid parentId, CopyRecipeRequest request);
    }
}
