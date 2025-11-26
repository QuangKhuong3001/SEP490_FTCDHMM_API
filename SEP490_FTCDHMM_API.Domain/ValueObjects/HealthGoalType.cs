using SEP490_FTCDHMM_API.Shared.Exceptions;

namespace SEP490_FTCDHMM_API.Domain.ValueObjects
{
    public record HealthGoalType(string Value)
    {
        public static readonly HealthGoalType SYSTEM = new("SYSTEM");
        public static readonly HealthGoalType CUSTOM = new("CUSTOM");

        public override string ToString() => Value;

        public static HealthGoalType From(string value)
        {
            return value.Trim().ToUpperInvariant() switch
            {
                "SYSTEM" => SYSTEM,
                "CUSTOM" => CUSTOM,
                _ => throw new AppException(AppResponseCode.INVALID_ACTION, "Loại mục tiêu sức khỏe không hợp lệ.")
            };
        }
    }
}
