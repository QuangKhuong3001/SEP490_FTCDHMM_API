namespace SEP490_FTCDHMM_API.Domain.Entities
{
    public class IngredientCategory
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public required string Name { get; set; }
        public ICollection<IngredientCategoryAssignment> IngredientCategoryAssignments { get; set; }
            = new List<IngredientCategoryAssignment>();
    }
}
