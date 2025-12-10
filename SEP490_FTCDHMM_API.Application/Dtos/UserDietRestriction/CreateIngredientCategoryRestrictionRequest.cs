namespace SEP490_FTCDHMM_API.Application.Dtos.UserDietRestriction
{
    public class CreateIngredientCategoryRestrictionRequest
    {
        public Guid IngredientCategoryId { get; set; }
        public string Type { get; set; } = string.Empty;
        public string? Notes { get; set; }
        public DateTime? ExpiredAtUtc { get; set; }
    }
}
