using SEP490_FTCDHMM_API.Application.Dtos.NutrientTargetDtos;

namespace SEP490_FTCDHMM_API.Application.Dtos.CustomHealthGoalDtos
{
    public class UpdateCustomHealthGoalRequest
    {
        public required string Name { get; set; }
        public string? Description { get; set; }
        public List<NutrientTargetRequest> Targets { get; set; } = new();
    }
}
