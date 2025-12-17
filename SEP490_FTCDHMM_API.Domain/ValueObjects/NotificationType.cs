using SEP490_FTCDHMM_API.Shared.Exceptions;

namespace SEP490_FTCDHMM_API.Domain.ValueObjects
{
    public record NotificationType(string Name)
    {
        public static readonly NotificationType System = new("SYSTEM");
        public static readonly NotificationType Comment = new("COMMENT");
        public static readonly NotificationType Mention = new("MENTION");
        public static readonly NotificationType Like = new("LIKE");
        public static readonly NotificationType Follow = new("FOLLOW");
        public static readonly NotificationType NewRecipe = new("NEWRECIPE");
        public static readonly NotificationType LockRecipe = new("LOCKRECIPE");
        public static readonly NotificationType DeleteRecipe = new("DELETERECIPE");
        public static readonly NotificationType ApproveRecipe = new("APPROVERECIPE");
        public static readonly NotificationType RejectRecipe = new("REJECTRECIPE");
        public static readonly NotificationType Reply = new("REPLY");
        public static readonly NotificationType Achievement = new("ACHIEVEMENT");

        public override string ToString() => Name;

        public static NotificationType From(string Name)
        {
            return Name.Trim().ToUpperInvariant() switch
            {
                "SYSTEM" => System,
                "COMMENT" => Comment,
                "LIKE" => Like,
                "FOLLOW" => Follow,
                "MENTION" => Mention,
                "REPLY" => Reply,
                "ACHIEVEMENT" => Achievement,
                "NEWRECIPE" => NewRecipe,
                "LOCKRECIPE" => LockRecipe,
                "DELETERECIPE" => DeleteRecipe,
                "APPROVERECIPE" => ApproveRecipe,
                "REJECTRECIPE" => RejectRecipe,
                _ => throw new AppException(AppResponseCode.INVALID_ACTION)
            };
        }
    }
}
