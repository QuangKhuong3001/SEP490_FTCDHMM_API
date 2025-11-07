namespace SEP490_FTCDHMM_API.Domain.Entities
{
    public class Label
    {
        public Guid Id { get; set; }
        public required string Name { get; set; }
        public required string ColorCode { get; set; }
        public bool IsDeleted { get; set; } = false;
        public ICollection<Recipe> Recipes { get; set; } = new List<Recipe>();

    }
}
