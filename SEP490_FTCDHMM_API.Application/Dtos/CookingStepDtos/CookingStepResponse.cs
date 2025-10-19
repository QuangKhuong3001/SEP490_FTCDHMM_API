namespace SEP490_FTCDHMM_API.Application.Dtos.CookingStepDtos
{
    public class CookingStepResponse
    {
        public Guid Id { get; set; }
        public required string Instruction { get; set; }
        public string? ImageUrl { get; set; }
        public required int StepOrder { get; set; }
    }
}
