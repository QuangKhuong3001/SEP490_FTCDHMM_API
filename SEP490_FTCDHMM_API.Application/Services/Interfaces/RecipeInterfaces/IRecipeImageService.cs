using SEP490_FTCDHMM_API.Application.Dtos.Common;
using SEP490_FTCDHMM_API.Application.Dtos.RecipeDtos.CookingStep;
using SEP490_FTCDHMM_API.Domain.Entities;

namespace SEP490_FTCDHMM_API.Application.Services.Interfaces.RecipeInterfaces
{
    public interface IRecipeImageService
    {
        Task SetRecipeImageAsync(Recipe recipe, FileUploadModel? file, Guid userId);
        Task ReplaceRecipeImageAsync(Recipe recipe, FileUploadModel? file, Guid userId);
        Task<List<CookingStep>> CreateCookingStepsAsync(IEnumerable<CookingStepRequest> steps, Recipe recipe, Guid userId);
        Task ReplaceCookingStepsAsync(Guid recipeId, IEnumerable<CookingStepRequest> newSteps, Guid userId);
    }
}
