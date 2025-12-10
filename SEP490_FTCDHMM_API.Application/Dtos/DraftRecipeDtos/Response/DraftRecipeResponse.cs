namespace SEP490_FTCDHMM_API.Application.Dtos.DraftRecipeDtos.Response
{
    public class DraftRecipeResponse
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; } = string.Empty;
        public string Difficulty { get; set; } = string.Empty;
        public int CookTime { get; set; }
        public string? ImageUrl { get; set; }
        public int? Ration { get; set; }
    }
}
