namespace SEP490_FTCDHMM_API.Application.Dtos.HealthGoalDtos
{
    public class HealthGoalCalculationResult
    {
        public double BMR { get; set; }
        public double TDEE { get; set; }
        public double TargetCalories { get; set; }

        public decimal ProteinGrams { get; set; }
        public decimal FatGrams { get; set; }
        public decimal CarbGrams { get; set; }

        public string Summary { get; set; } = string.Empty;
    }
}
