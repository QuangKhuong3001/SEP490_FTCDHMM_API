namespace SEP490_FTCDHMM_API.Application.Dtos.HealthGoalDtos
{
    public class HealthGoalCalculationRequest
    {
        public double WeightKg { get; set; }
        public double HeightCm { get; set; }
        public int Age { get; set; }
        public string Gender { get; set; } = string.Empty;
        public string GoalType { get; set; } = string.Empty;
        public double ActivityLevel { get; set; } = 1.2;
    }
}
