namespace SEP490_FTCDHMM_API.Domain.ValueObjects
{
    public record PermissionValue(string Domain, string Action)
    {
        public override string ToString() => $"{Domain}:{Action}";

        public static readonly PermissionValue Moderator_Create = new("ModeratorManagement", "Create");
        public static readonly PermissionValue Moderator_View = new("ModeratorManagement", "View");
        public static readonly PermissionValue Moderator_Update = new("ModeratorManagement", "Update");
        public static readonly PermissionValue Moderator_Delete = new("ModeratorManagement", "Delete");

        public static readonly PermissionValue Customer_Create = new("CustomerManagement", "Create");
        public static readonly PermissionValue Customer_View = new("CustomerManagement", "View");
        public static readonly PermissionValue Customer_Update = new("CustomerManagement", "Update");
        public static readonly PermissionValue Customer_Delete = new("CustomerManagement", "Delete");

        public static IEnumerable<PermissionValue> All => new[]
        {
            Moderator_Create,
            Moderator_View,
            Moderator_Update,
            Moderator_Delete,
            Customer_Create,
            Customer_Update
        };
    }
}
