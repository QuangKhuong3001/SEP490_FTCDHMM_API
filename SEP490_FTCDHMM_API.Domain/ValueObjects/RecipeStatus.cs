using SEP490_FTCDHMM_API.Shared.Exceptions;

namespace SEP490_FTCDHMM_API.Domain.ValueObjects
{
    public record RecipeStatus(string Value)
    {
        public static readonly RecipeStatus Posted = new("POSTED");
        public static readonly RecipeStatus Locked = new("LOCKED");
        public static readonly RecipeStatus Pending = new("PENDING");
        public static readonly RecipeStatus Deleted = new("DELETED");

        public static RecipeStatus From(string value)
        {
            return value.Trim().ToUpperInvariant() switch
            {
                "POSTED" => Posted,
                "LOCKED" => Locked,
                "DELETED" => Deleted,
                "PENDING" => Pending,
                _ => throw new AppException(AppResponseCode.INVALID_ACTION)
            };
        }
    }
}
