namespace SEP490_FTCDHMM_API.Domain.Entities
{
    public class IngredientCategory
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public required string Name { get; set; }
        public bool isDeleted { get; set; } = false;
        public ICollection<Ingredient> Ingredients { get; set; } = new List<Ingredient>();

    }
}
