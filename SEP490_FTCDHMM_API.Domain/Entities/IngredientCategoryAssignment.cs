namespace SEP490_FTCDHMM_API.Domain.Entities
{
    public class IngredientCategoryAssignment
    {
        public Guid IngredientId { get; set; } = Guid.NewGuid();
        public Ingredient Ingredient { get; set; } = null!;

        public Guid CategoryId { get; set; }
        public IngredientCategory Category { get; set; } = null!;
    }
}
