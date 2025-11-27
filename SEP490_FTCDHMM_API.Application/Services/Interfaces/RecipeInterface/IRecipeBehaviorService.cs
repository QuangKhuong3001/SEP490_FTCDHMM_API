using SEP490_FTCDHMM_API.Domain.Entities;

namespace SEP490_FTCDHMM_API.Application.Services.Interfaces.RecipeInterface
{
    public interface IRecipeBehaviorService
    {
        Task RecordViewAsync(Guid userId, Recipe recipe);

        Task RecordFarvoriteAsync(Guid userId, Guid recipeId);
        Task RecordUnFavoriteAsync(Guid userId, Guid recipeId);

        Task RecordSaveAsync(Guid userId, Guid recipeId);
        Task RecordUnsaveAsync(Guid userId, Guid recipeId);

        Task RecordRatingAsync(Guid userId, Guid recipeId, int rating);
        Task RecordUpdateRatingAsync(Guid userId, Guid recipeId, int newRating);
    }

}
