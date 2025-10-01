namespace SEP490_FTCDHMM_API.Domain.ValueObjects
{
    public record RoleValue(string Name)
    {
        public static readonly RoleValue Admin = new("Admin");
        public static readonly RoleValue Moderator = new("Moderator");
        public static readonly RoleValue Customer = new("Customer");

        public static IEnumerable<RoleValue> All => new[]
        {
            Admin,
            Moderator,
            Customer
        };

        public override string ToString() => Name;
    }
}
