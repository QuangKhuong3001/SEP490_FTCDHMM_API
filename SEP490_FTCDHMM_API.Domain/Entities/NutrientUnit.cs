namespace SEP490_FTCDHMM_API.Domain.Entities
{
    public class NutrientUnit
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public required string Name { get; set; }
        public string Symbol { get; set; } = string.Empty;
        public string? Description { get; set; }

        public ICollection<Nutrient> Nutrients { get; set; } = new List<Nutrient>();
    }
}
