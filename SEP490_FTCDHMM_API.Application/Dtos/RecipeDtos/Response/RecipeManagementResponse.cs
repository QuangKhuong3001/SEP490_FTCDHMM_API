using SEP490_FTCDHMM_API.Application.Dtos.UserDtos;
using SEP490_FTCDHMM_API.Domain.ValueObjects;

namespace SEP490_FTCDHMM_API.Application.Dtos.RecipeDtos.Response
{
    public class RecipeManagementResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; } = string.Empty;
        public UserInteractionResponse Author { get; set; } = new();
        public DifficultyValue Difficulty { get; set; } = DifficultyValue.Medium;
        public decimal CookTime { get; set; }
        public int Ration { get; set; }
        public string? ImageUrl { get; set; }
        public RecipeStatus Status { get; set; } = RecipeStatus.Pending;
        public DateTime CreatedAtUtc { get; set; }
        public DateTime UpdatedAtUtc { get; set; }
        public string Reason { get; set; } = string.Empty;
    }
}
