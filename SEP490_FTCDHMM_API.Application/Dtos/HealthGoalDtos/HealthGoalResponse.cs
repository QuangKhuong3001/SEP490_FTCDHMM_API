using SEP490_FTCDHMM_API.Application.Dtos.NutrientTargetDtos;

namespace SEP490_FTCDHMM_API.Application.Dtos.HealthGoalDtos
{
    public class HealthGoalResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public List<NutrientTargetResponse> Targets { get; set; } = new();
    }
}
