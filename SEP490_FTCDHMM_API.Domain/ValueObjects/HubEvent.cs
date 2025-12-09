using SEP490_FTCDHMM_API.Shared.Exceptions;

namespace SEP490_FTCDHMM_API.Domain.ValueObjects
{
    public record HubEvent(string Value)
    {
        public static readonly HubEvent CommentAdded = new("COMMENTADDED");
        public static readonly HubEvent CommentUpdated = new("COMMENTUPDATED");
        public static readonly HubEvent CommentDeleted = new("COMMENTDELETED");
        public static readonly HubEvent RatingUpdated = new("RATINGUPDATED");
        public static readonly HubEvent RatingDeleted = new("RATINGDELETED");
        public static readonly HubEvent Notification = new("NOTIFICATION");
        public static readonly HubEvent ReadNotification = new("READNOTIFICATION");

        public override string ToString() => Value;

        public static HubEvent From(string value)
        {
            return value.Trim().ToUpperInvariant() switch
            {
                "COMMENTADDED" => CommentAdded,
                "COMMENTDELETED" => CommentDeleted,
                "RATINGUPDATED" => RatingUpdated,
                "RATINGDELETED" => RatingDeleted,
                "NOTIFICATION" => Notification,
                "COMMENTUPDATED" => CommentUpdated,
                "READNOTIFICATION" => ReadNotification,
                _ => throw new AppException(AppResponseCode.INVALID_ACTION, "Không tồn tại hub event này")
            };
        }
    }
}
