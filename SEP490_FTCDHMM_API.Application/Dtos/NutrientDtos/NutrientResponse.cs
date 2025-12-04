namespace SEP490_FTCDHMM_API.Application.Dtos.NutrientDtos
{
    public class NutrientResponse
    {
        public Guid Id { get; set; }
        public string VietnameseName { get; set; } = string.Empty;
        public string Unit { get; set; } = string.Empty;
        public decimal? MinValue { get; set; }
        public decimal? MaxValue { get; set; }
        public decimal MedianValue { get; set; }
    }
}
