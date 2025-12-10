using SEP490_FTCDHMM_API.Shared.Exceptions;

namespace SEP490_FTCDHMM_API.Domain.ValueObjects
{
    public record ReportObjectType(string Value)
    {
        public static readonly ReportObjectType Recipe = new("RECIPE");
        public static readonly ReportObjectType User = new("USER");
        public static readonly ReportObjectType Comment = new("COMMENT");
        public static readonly ReportObjectType Rating = new("RATING");

        public override string ToString() => Value;

        public static ReportObjectType From(string value)
        {
            return value.Trim().ToUpperInvariant() switch
            {
                "RECIPE" => Recipe,
                "USER" => User,
                "COMMENT" => Comment,
                "RATING" => Rating,
                _ => throw new AppException(AppResponseCode.INVALID_ACTION, "Loại đối tượng report không hợp lệ.")
            };
        }
    }
}