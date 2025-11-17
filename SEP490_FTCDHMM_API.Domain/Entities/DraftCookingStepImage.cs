namespace SEP490_FTCDHMM_API.Domain.Entities
{
    public class DraftCookingStepImage
    {
        public Guid Id { get; set; }

        public Guid DraftCookingStepId { get; set; }
        public DraftCookingStep DraftCookingStep { get; set; } = null!;

        public Guid ImageId { get; set; }
        public Image Image { get; set; } = null!;
        public required int ImageOrder { get; set; }
    }
}
