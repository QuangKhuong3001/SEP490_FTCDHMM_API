namespace SEP490_FTCDHMM_API.Domain.ValueObjects
{
    public record HubEvent(string Value)
    {
        public static readonly HubEvent ReceiveComment = new("ReceiveComment");
        public static readonly HubEvent ReceiveRatingUpdate = new("ReceiveRatingUpdate");
        public static readonly HubEvent ReceiveNotification = new("ReceiveNotification");

        public override string ToString() => Value;

        public static HubEvent From(string value)
        {
            return value switch
            {
                "ReceiveComment" => ReceiveComment,
                "ReceiveRatingUpdate" => ReceiveRatingUpdate,
                "ReceiveNotification" => ReceiveNotification,
                _ => new HubEvent(value)
            };
        }
    }
}