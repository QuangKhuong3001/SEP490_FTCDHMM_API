namespace SEP490_FTCDHMM_API.Api.Dtos.MealDtos
{
    public class MealAnalyzeRequest
    {
        public Guid MealSlotId { get; set; }
        public List<Guid> CurrentRecipeIds { get; set; } = new();
        public int SuggestionLimit { get; set; } = 10;
    }
}
