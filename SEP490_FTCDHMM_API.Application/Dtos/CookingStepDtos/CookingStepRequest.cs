using SEP490_FTCDHMM_API.Application.Dtos.CookingStepImageDtos;

namespace SEP490_FTCDHMM_API.Application.Dtos.CookingStepDtos
{
    public class CookingStepRequest
    {
        public required string Instruction { get; set; }
        public List<CookingStepImageRequest> Images { get; set; } = new();
        public required int StepOrder { get; set; }
    }
}
