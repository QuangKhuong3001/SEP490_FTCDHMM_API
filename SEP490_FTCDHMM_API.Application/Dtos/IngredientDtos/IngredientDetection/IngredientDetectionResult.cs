namespace SEP490_FTCDHMM_API.Application.Dtos.IngredientDtos.IngredientDetection
{
    public class IngredientDetectionResult
    {
        public string Id { get; set; } = string.Empty;
        public string Ingredient { get; set; } = string.Empty;
        public double Confidence { get; set; }
    }
}
