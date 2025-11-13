namespace SEP490_FTCDHMM_API.Domain.Entities
{
    public class CookingStepImage
    {
        public Guid Id { get; set; }

        public Guid CookingStepId { get; set; }
        public CookingStep CookingStep { get; set; } = null!;

        public Guid ImageId { get; set; }
        public Image Image { get; set; } = null!;
        public required int ImageOrder { get; set; }
    }
}
