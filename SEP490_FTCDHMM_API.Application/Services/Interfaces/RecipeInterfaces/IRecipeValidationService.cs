using SEP490_FTCDHMM_API.Application.Dtos.RecipeDtos.CookingStep;
using SEP490_FTCDHMM_API.Domain.Entities;

namespace SEP490_FTCDHMM_API.Application.Services.Interfaces.RecipeInterfaces
{
    public interface IRecipeValidationService
    {
        Task ValidateUserExistsAsync(Guid userId);
        Task ValidateLabelsAsync(IEnumerable<Guid> labelIds);
        Task ValidateIngredientsAsync(IEnumerable<Guid> ingredientIds);
        Task ValidateCookingStepsAsync(IEnumerable<CookingStepRequest> steps);
        Task ValidateTaggedUsersAsync(Guid authorId, IEnumerable<Guid> taggedUserIds);
        Task ValidateRecipeOwnerAsync(Guid userId, Recipe recipe);
    }

}
