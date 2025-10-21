namespace SEP490_FTCDHMM_API.Application.Dtos.IngredientDetectionDtos
{
    public class IngredientDetectionResult
    {
        public string Ingredient { get; set; } = string.Empty;
        public double Confidence { get; set; }
    }
}
