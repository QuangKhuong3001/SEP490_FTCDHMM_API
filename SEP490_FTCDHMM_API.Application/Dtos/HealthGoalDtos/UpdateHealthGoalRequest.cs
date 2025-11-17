using SEP490_FTCDHMM_API.Application.Dtos.NutrientDtos.NutrientTarget;

namespace SEP490_FTCDHMM_API.Application.Dtos.HealthGoalDtos
{
    public class UpdateHealthGoalRequest
    {
        public string? Description { get; set; }
        public List<NutrientTargetRequest> Targets { get; set; } = new();
    }
}
