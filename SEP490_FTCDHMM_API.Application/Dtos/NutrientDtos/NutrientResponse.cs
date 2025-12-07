namespace SEP490_FTCDHMM_API.Application.Dtos.NutrientDtos
{
    public class NutrientResponse
    {
        public Guid Id { get; set; }
        public string VietnameseName { get; set; } = string.Empty;
        public string Unit { get; set; } = string.Empty;
        public decimal Value { get; set; }
    }
}
