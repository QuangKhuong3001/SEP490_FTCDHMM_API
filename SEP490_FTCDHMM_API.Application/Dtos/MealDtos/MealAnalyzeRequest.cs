namespace SEP490_FTCDHMM_API.Application.Dtos.MealDtos
{
    public class MealAnalyzeRequest
    {
        public List<Guid> CurrentRecipeIds { get; set; } = new();
        public int SuggestionLimit { get; set; } = 10;
    }
}
