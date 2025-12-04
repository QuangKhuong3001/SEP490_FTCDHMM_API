namespace SEP490_FTCDHMM_API.Api.Dtos.RecipeDtos.UserSaveRecipe
{
    public class SaveRecipeFilterRequest
    {
        public string? Keyword { get; set; }
        public required RecipePaginationParams PaginationParams { get; set; }
    }
}
