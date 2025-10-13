namespace SEP490_FTCDHMM_API.Application.Dtos.NutrientDtos
{
    public class NutrientRequest
    {
        public Guid NutrientId { get; set; }
        public decimal? Min { get; set; }
        public decimal? Max { get; set; }
        public decimal Median { get; set; }
    }
}
