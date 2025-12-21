using SEP490_FTCDHMM_API.Application.Dtos.Common;
using SEP490_FTCDHMM_API.Application.Dtos.RecipeDtos.CookingStep;
using SEP490_FTCDHMM_API.Domain.Entities;

namespace SEP490_FTCDHMM_API.Application.Services.Interfaces.RecipeInterfaces
{
    public interface IRecipeImageService
    {
        Task SetRecipeImageAsync(Recipe recipe, FileUploadModel? file, string? url);
        Task ReplaceRecipeImageAsync(Recipe recipe, FileUploadModel? file);
        Task<List<CookingStep>> CreateCookingStepsAsync(IEnumerable<CookingStepRequest> steps, Recipe recipe);
        Task<List<CookingStep>> CreateCookingStepsWithDraftImagesAsync(IEnumerable<CookingStepRequest> steps, Recipe recipe, Dictionary<(int stepOrder, int imageOrder), Guid> draftStepImageMap);
        Task ReplaceCookingStepsAsync(Guid recipeId, IEnumerable<CookingStepRequest> newSteps);
        Task DeleteImageAsync(Guid? imageId);
    }
}
