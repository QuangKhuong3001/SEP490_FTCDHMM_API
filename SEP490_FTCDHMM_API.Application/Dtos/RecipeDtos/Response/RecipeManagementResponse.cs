using SEP490_FTCDHMM_API.Application.Dtos.UserDtos;
using SEP490_FTCDHMM_API.Domain.ValueObjects;

namespace SEP490_FTCDHMM_API.Application.Dtos.RecipeDtos.Response
{
    public class RecipeManagementResponse
    {
        public Guid Id { get; set; }
        public required string Name { get; set; }
        public string? Description { get; set; } = string.Empty;
        public required UserInteractionResponse Author { get; set; }
        public required DifficultyValue Difficulty { get; set; }
        public decimal CookTime { get; set; }
        public required int Ration { get; set; }
        public string? ImageUrl { get; set; }
        public required RecipeStatus Status { get; set; }
        public DateTime CreatedAtUtc { get; set; }
        public DateTime UpdatedAtUtc { get; set; }
        public string Reason { get; set; } = string.Empty;
    }
}
