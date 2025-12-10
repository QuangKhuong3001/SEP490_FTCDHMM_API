namespace SEP490_FTCDHMM_API.Domain.Entities
{
    public class Nutrient
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; } = string.Empty;
        public string VietnameseName { get; set; } = string.Empty;

        public string? Description { get; set; }
        public bool IsMacroNutrient { get; set; } = false;
        public Guid UnitId { get; set; }
        public NutrientUnit Unit { get; set; } = null!;
        public ICollection<IngredientNutrient> IngredientNutrients { get; set; } = new List<IngredientNutrient>();
    }
}
