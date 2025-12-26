namespace SEP490_FTCDHMM_API.Domain.Entities
{
    public class UserMealSlot
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string Name { get; set; } = null!;
        public decimal EnergyPercent { get; set; }
        public int OrderIndex { get; set; }
        public AppUser AppUser { get; set; } = null!;
    }
}
