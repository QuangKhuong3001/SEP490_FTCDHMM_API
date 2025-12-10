namespace SEP490_FTCDHMM_API.Application.Dtos.UserDietRestriction
{
    public class CreateIngredientRestrictionRequest
    {
        public Guid IngredientId { get; set; }
        public string Type { get; set; } = string.Empty;
        public string? Notes { get; set; }
        public DateTime? ExpiredAtUtc { get; set; }
    }
}
