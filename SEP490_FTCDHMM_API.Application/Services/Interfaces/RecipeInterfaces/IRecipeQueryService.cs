using SEP490_FTCDHMM_API.Application.Dtos.Common;
using SEP490_FTCDHMM_API.Application.Dtos.RecipeDtos;
using SEP490_FTCDHMM_API.Application.Dtos.RecipeDtos.Rating;
using SEP490_FTCDHMM_API.Application.Dtos.RecipeDtos.Response;
using SEP490_FTCDHMM_API.Application.Dtos.RecipeDtos.UserSaveRecipe;

namespace SEP490_FTCDHMM_API.Application.Services.Interfaces.RecipeInterfaces
{
    public interface IRecipeQueryService
    {
        Task<PagedResult<RecipeResponse>> GetRecipesAsync(RecipeFilterRequest request);
        Task<RecipeDetailsResponse> GetRecipeDetailsAsync(Guid? userId, Guid recipeId);
        Task<PagedResult<RecipeResponse>> GetSavedRecipesAsync(Guid userId, SaveRecipeFilterRequest request);
        Task<PagedResult<MyRecipeResponse>> GetRecipesByUserIdAsync(Guid userId, RecipePaginationParams paginationParams);
        Task<PagedResult<MyRecipeResponse>> GetRecipesByUserNameAsync(string userName, RecipePaginationParams paginationParams);
        Task<RecipeRatingResponse> GetRecipeRatingsAsync(Guid? userId, Guid recipeId);
        Task<PagedResult<RatingDetailsResponse>> GetRecipeRatingDetailsAsync(Guid? userId, Guid recipeId, RecipePaginationParams request);
        Task<PagedResult<RecipeResponse>> GetRecipeHistoriesAsync(Guid userId, RecipePaginationParams request);
        Task<PagedResult<RecipeManagementResponse>> GetRecipePendingsAsync(PaginationParams request);
        Task<PagedResult<RecipeManagementResponse>> GetRecipePendingsByUserIdAsync(Guid userId, PaginationParams request);
    }
}
