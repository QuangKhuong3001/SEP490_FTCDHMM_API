namespace SEP490_FTCDHMM_API.Domain.Entities
{
    public class Label
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string UpperName { get; set; } = string.Empty;
        public string NormalizedName { get; set; } = string.Empty;
        public string ColorCode { get; set; } = string.Empty;
        public bool IsDeleted { get; set; } = false;
        public ICollection<Recipe> Recipes { get; set; } = new List<Recipe>();

    }
}
