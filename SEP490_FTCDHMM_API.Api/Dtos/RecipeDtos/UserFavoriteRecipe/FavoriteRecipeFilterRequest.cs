using SEP490_FTCDHMM_API.Application.Dtos.Common;

namespace SEP490_FTCDHMM_API.Api.Dtos.RecipeDtos.UserFavoriteRecipe
{
    public class FavoriteRecipeFilterRequest
    {
        public string? Keyword { get; set; }
        public required RecipePaginationParams PaginationParams { get; set; }
    }
}
