namespace SEP490_FTCDHMM_API.Domain.Entities
{
    public class CookingStep
    {
        public Guid Id { get; set; }
        public required string Instruction { get; set; }
        public List<CookingStepImage> CookingStepImages { get; set; } = new();

        public required int StepOrder { get; set; }
        public Guid RecipeId { get; set; }
        public Recipe Recipe { get; set; } = null!;
    }
}
