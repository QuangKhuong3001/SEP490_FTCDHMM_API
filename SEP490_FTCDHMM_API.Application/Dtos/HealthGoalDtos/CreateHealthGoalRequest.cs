using SEP490_FTCDHMM_API.Application.Dtos.NutrientTargetDtos;

namespace SEP490_FTCDHMM_API.Application.Dtos.HealthGoalDtos
{
    public class CreateHealthGoalRequest
    {
        public required string Name { get; set; }
        public string? Description { get; set; }
        public List<NutrientTargetDto> Targets { get; set; } = new();
    }
}
