namespace SEP490_FTCDHMM_API.Application.Dtos.NutrientTargetDtos
{
    public class NutrientTargetResponse
    {
        public Guid NutrientId { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal MinValue { get; set; }
        public decimal MaxValue { get; set; }
    }
}
