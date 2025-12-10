namespace SEP490_FTCDHMM_API.Api.Dtos.RecipeDtos.UserFavoriteRecipe
{
    public class FavoriteRecipeFilterRequest
    {
        public string? Keyword { get; set; }
        public RecipePaginationParams PaginationParams { get; set; } = new();
    }
}
