namespace SEP490_FTCDHMM_API.Application.Dtos.NutrientDtos
{
    public class NutrientResponse
    {
        public string Name { get; set; } = string.Empty;
        public string Unit { get; set; } = string.Empty;
        public decimal? Min { get; set; }
        public decimal? Max { get; set; }
        public decimal Median { get; set; }
    }
}
