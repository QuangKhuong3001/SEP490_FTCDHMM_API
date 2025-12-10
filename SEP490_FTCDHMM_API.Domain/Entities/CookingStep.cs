namespace SEP490_FTCDHMM_API.Domain.Entities
{
    public class CookingStep
    {
        public Guid Id { get; set; }
        public string Instruction { get; set; } = string.Empty;
        public List<CookingStepImage> CookingStepImages { get; set; } = new();

        public int StepOrder { get; set; }
        public Guid RecipeId { get; set; }
        public Recipe Recipe { get; set; } = null!;
    }
}
