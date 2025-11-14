using SEP490_FTCDHMM_API.Shared.Exceptions;

namespace SEP490_FTCDHMM_API.Domain.ValueObjects
{
    public record HubEvent(string Value)
    {
        public static readonly HubEvent CommentAdded = new("CommentAdded");
        public static readonly HubEvent CommentDeleted = new("CommentDeleted");
        public static readonly HubEvent RatingUpdated = new("RatingUpdated");
        public static readonly HubEvent RatingDeleted = new("RatingDeleted");
        public static readonly HubEvent Notification = new("Notification");

        public override string ToString() => Value;

        public static HubEvent From(string value)
        {
            return value switch
            {
                "CommentAdded" => CommentAdded,
                "CommentDeleted" => CommentDeleted,
                "RatingUpdate" => RatingUpdated,
                "RatingDelete" => RatingDeleted,
                "Notification" => Notification,
                _ => throw new AppException(AppResponseCode.INVALID_ACTION, "Không tồn tại hub event này")
            };
        }
    }
}
