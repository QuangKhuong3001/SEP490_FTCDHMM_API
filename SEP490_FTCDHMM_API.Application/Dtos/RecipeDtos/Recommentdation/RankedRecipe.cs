namespace SEP490_FTCDHMM_API.Application.Dtos.RecipeDtos.Recommentdation
{
    public class RankedRecipe
    {
        public Guid Id { get; set; }
        public double Score { get; set; }
        public DateTime UpdatedAtUtc { get; set; }
    }
}
