using SEP490_FTCDHMM_API.Application.Dtos.NutrientDtos.NutrientTarget;

namespace SEP490_FTCDHMM_API.Application.Dtos.UserHealthGoalDtos
{
    public class UserHealthGoalResponse
    {
        public Guid? HealthGoalId { get; set; }
        public Guid? CustomHealthGoalId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime? StartedAtUtc { get; set; }
        public DateTime? ExpiredAtUtc { get; set; }
        public List<NutrientTargetResponse> Targets { get; set; } = new();
    }
}
