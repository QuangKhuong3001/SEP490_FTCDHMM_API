using SEP490_FTCDHMM_API.Application.Dtos.Common;

namespace SEP490_FTCDHMM_API.Application.Dtos.UserSaveRecipeDtos
{
    public class SaveRecipeFilterRequest
    {
        public string? Keyword { get; set; }
        public required PaginationParams PaginationParams { get; set; }
    }
}
