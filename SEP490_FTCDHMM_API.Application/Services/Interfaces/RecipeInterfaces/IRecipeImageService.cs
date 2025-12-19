using SEP490_FTCDHMM_API.Application.Dtos.Common;
using SEP490_FTCDHMM_API.Application.Dtos.RecipeDtos.CookingStep;
using SEP490_FTCDHMM_API.Domain.Entities;

namespace SEP490_FTCDHMM_API.Application.Services.Interfaces.RecipeInterfaces
{
    public interface IRecipeImageService
    {
        Task SetRecipeImageAsync(Recipe recipe, FileUploadModel? file, Guid? existingImageId = null);
        Task ReplaceRecipeImageAsync(Recipe recipe, FileUploadModel? file, Guid? existingImageId = null);
        Task<List<CookingStep>> CreateCookingStepsAsync(IEnumerable<CookingStepRequest> steps, Recipe recipe);
        Task ReplaceCookingStepsAsync(Guid recipeId, IEnumerable<CookingStepRequest> newSteps);

        /// Copy main image from parent recipe to the new recipe
        Task CopyMainImageFromParentAsync(Recipe recipe, Recipe parentRecipe);

        /// Create cooking steps with images copied from parent recipe's corresponding steps
        Task<List<CookingStep>> CreateCookingStepsWithCopyFromParentAsync(
            IEnumerable<CookingStepRequest> steps,
            Recipe recipe,
            IEnumerable<CookingStep> parentSteps);
    }
}
