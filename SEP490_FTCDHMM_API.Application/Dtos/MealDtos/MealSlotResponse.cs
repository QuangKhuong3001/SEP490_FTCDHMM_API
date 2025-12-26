namespace SEP490_FTCDHMM_API.Application.Dtos.MealDtos
{
    public class MealSlotResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public decimal EnergyPercent { get; set; }
        public int OrderIndex { get; set; }
    }
}
