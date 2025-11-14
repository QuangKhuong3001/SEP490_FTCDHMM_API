using SEP490_FTCDHMM_API.Application.Dtos.Common;
using SEP490_FTCDHMM_API.Application.Dtos.RatingDtos;
using SEP490_FTCDHMM_API.Application.Dtos.RecipeDtos;
using SEP490_FTCDHMM_API.Application.Dtos.UserFavoriteRecipeDtos;
using SEP490_FTCDHMM_API.Application.Dtos.UserSaveRecipeDtos;

namespace SEP490_FTCDHMM_API.Application.Services.Interfaces
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
        Task<PagedResult<MyRecipeResponse>> GetRecipeByUserId(Guid userId, PaginationParams paginationParams);
        Task<double> GetAverageScore(Guid recipeId);
        Task<PagedResult<RatingResponse>> GetRaiting(Guid recipeId, PaginationParams request);
        Task<PagedResult<RecipeResponse>> GetHistory(Guid userId, PaginationParams request);
    }
}
