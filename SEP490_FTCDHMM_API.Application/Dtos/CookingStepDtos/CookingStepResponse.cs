using SEP490_FTCDHMM_API.Application.Dtos.CookingStepImageDtos;

namespace SEP490_FTCDHMM_API.Application.Dtos.CookingStepDtos
{
    public class CookingStepResponse
    {
        public Guid Id { get; set; }
        public required string Instruction { get; set; }
        public List<CookingStepImageResponse> CookingStepImages { get; set; } = new();
        public required int StepOrder { get; set; }
    }
}
