using SEP490_FTCDHMM_API.Application.Dtos.Common;
using SEP490_FTCDHMM_API.Application.Dtos.RecipeDtos;
using SEP490_FTCDHMM_API.Application.Dtos.RecipeDtos.Rating;
using SEP490_FTCDHMM_API.Application.Dtos.RecipeDtos.Response;
using SEP490_FTCDHMM_API.Application.Dtos.RecipeDtos.UserFavoriteRecipe;
using SEP490_FTCDHMM_API.Application.Dtos.RecipeDtos.UserSaveRecipe;

namespace SEP490_FTCDHMM_API.Application.Services.Interfaces.RecipeInterface
{
    public interface IRecipeQueryService
    {
        Task<PagedResult<RecipeResponse>> GetAllRecipesAsync(RecipeFilterRequest request);
        Task<RecipeDetailsResponse> GetRecipeDetailsAsync(Guid userId, Guid recipeId);
        Task<PagedResult<RecipeResponse>> GetFavoriteListAsync(Guid userId, FavoriteRecipeFilterRequest request);
        Task<PagedResult<RecipeResponse>> GetSavedListAsync(Guid userId, SaveRecipeFilterRequest request);
        Task<PagedResult<MyRecipeResponse>> GetRecipeByUserIdAsync(Guid userId, RecipePaginationParams paginationParams);
        Task<PagedResult<MyRecipeResponse>> GetRecipeByUserNameAsync(string userName, RecipePaginationParams paginationParams);
        Task<RecipeRatingResponse> GetRecipeRatingAsync(Guid userId, Guid recipeId);
        Task<PagedResult<RatingDetailsResponse>> GetRatingDetailsAsync(Guid userId, Guid recipeId, RecipePaginationParams request);
        Task<PagedResult<RecipeResponse>> GetHistoryAsync(Guid userId, RecipePaginationParams request);
        Task<PagedResult<RecipeManagementResponse>> GetPendingListAsync(PaginationParams request);
        Task<PagedResult<RecipeManagementResponse>> GetUnPostedListAsync(Guid userId, PaginationParams request);
    }
}
