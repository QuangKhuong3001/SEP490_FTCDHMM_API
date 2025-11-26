using SEP490_FTCDHMM_API.Application.Dtos.Common;
using SEP490_FTCDHMM_API.Application.Dtos.RecipeDtos;
using SEP490_FTCDHMM_API.Application.Dtos.RecipeDtos.Rating;
using SEP490_FTCDHMM_API.Application.Dtos.RecipeDtos.UserFavoriteRecipe;
using SEP490_FTCDHMM_API.Application.Dtos.RecipeDtos.UserSaveRecipe;

namespace SEP490_FTCDHMM_API.Application.Services.Interfaces.RecipeInterface
{
    public interface IRecipeService
    {
        Task CreatRecipe(Guid userId, CreateRecipeRequest request);
        Task UpdateRecipe(Guid userId, Guid recipeId, UpdateRecipeRequest request);
        Task DeleteRecipe(Guid userId, Guid recipeId);
        Task<PagedResult<RecipeResponse>> GetAllRecipes(RecipeFilterRequest request);
        Task<RecipeDetailsResponse> GetRecipeDetails(Guid userId, Guid recipeId);
        Task<PagedResult<RecipeResponse>> GetFavoriteList(Guid userId, FavoriteRecipeFilterRequest request);
        Task AddToFavorite(Guid userId, Guid recipeId);
        Task RemoveFromFavorite(Guid userId, Guid recipeId);
        Task<PagedResult<RecipeResponse>> GetSavedList(Guid userId, SaveRecipeFilterRequest request);
        Task SaveRecipe(Guid userId, Guid recipeId);
        Task UnsaveRecipe(Guid userId, Guid recipeId);
        Task<PagedResult<MyRecipeResponse>> GetRecipeByUserId(Guid userId, RecipePaginationParams paginationParams);
        Task<PagedResult<MyRecipeResponse>> GetRecipeByUserName(string userName, RecipePaginationParams paginationParams);
        Task<RecipeRatingResponse> GetRecipeRating(Guid recipeId);
        Task<PagedResult<RatingDetailsResponse>> GetRatingDetails(Guid recipeId, RecipePaginationParams request);
        Task<PagedResult<RecipeResponse>> GetHistory(Guid userId, RecipePaginationParams request);
    }
}
