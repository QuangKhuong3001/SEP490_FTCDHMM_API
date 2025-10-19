using Microsoft.AspNetCore.Http;

namespace SEP490_FTCDHMM_API.Application.Dtos.CookingStepDtos
{
    public class CookingStepRequest
    {
        public required string Instruction { get; set; }
        public IFormFile? Image { get; set; }
        public required int StepOrder { get; set; }
    }
}
