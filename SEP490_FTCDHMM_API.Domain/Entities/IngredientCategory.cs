namespace SEP490_FTCDHMM_API.Domain.Entities
{
    public class IngredientCategory
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; } = string.Empty;
        public string UpperName { get; set; } = string.Empty;
        public string NormalizedName { get; set; } = string.Empty;
        public ICollection<Ingredient> Ingredients { get; set; } = new List<Ingredient>();

    }
}
