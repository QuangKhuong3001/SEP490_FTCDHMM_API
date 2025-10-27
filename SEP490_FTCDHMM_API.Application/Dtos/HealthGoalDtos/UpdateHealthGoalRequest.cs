using SEP490_FTCDHMM_API.Application.Dtos.NutrientTargetDtos;

namespace SEP490_FTCDHMM_API.Application.Dtos.HealthGoalDtos
{
    public class UpdateHealthGoalRequest
    {
        public string? Description { get; set; }
        public List<NutrientTargetDto> Targets { get; set; } = new();
    }
}
