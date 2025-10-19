namespace SEP490_FTCDHMM_API.Domain.Entities
{
    public class CookingStep
    {
        public Guid Id { get; set; }
        public required string Instruction { get; set; }
        public Guid? ImageId { get; set; }
        public Image? Image { get; set; }

        public required int StepOrder { get; set; }
        public Guid RecipeId { get; set; }
        public Recipe Recipe { get; set; } = null!;
    }
}
