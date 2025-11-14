namespace SEP490_FTCDHMM_API.Application.Dtos.CookingStepImageDtos
{
    public class CookingStepImageResponse
    {
        public Guid Id { get; set; }
        public string? ImageUrl { get; set; }
        public int ImageOrder { get; set; }
    }
}
