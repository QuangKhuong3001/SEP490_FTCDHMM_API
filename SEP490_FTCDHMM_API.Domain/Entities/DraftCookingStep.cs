namespace SEP490_FTCDHMM_API.Domain.Entities
{
    public class DraftCookingStep
    {
        public Guid Id { get; set; }
        public string? Instruction { get; set; }
        public List<DraftCookingStepImage> DraftCookingStepImages { get; set; } = new();
        public int StepOrder { get; set; }
        public Guid DraftRecipeId { get; set; }
        public DraftRecipe DraftRecipe { get; set; } = null!;
    }
}
