namespace SEP490_FTCDHMM_API.Application.Dtos.NutrientTargetDtos
{
    public class NutrientTargetDto
    {
        public Guid NutrientId { get; set; }
        public decimal MinValue { get; set; }
        public decimal MaxValue { get; set; }
    }

}
