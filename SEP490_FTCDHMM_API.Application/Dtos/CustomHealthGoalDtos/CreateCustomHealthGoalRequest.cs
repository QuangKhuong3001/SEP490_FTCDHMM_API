
using SEP490_FTCDHMM_API.Application.Dtos.NutrientDtos.NutrientTarget;

namespace SEP490_FTCDHMM_API.Application.Dtos.CustomHealthGoalDtos
{
    public class CreateCustomHealthGoalRequest
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public List<NutrientTargetRequest> Targets { get; set; } = new();
    }
}
