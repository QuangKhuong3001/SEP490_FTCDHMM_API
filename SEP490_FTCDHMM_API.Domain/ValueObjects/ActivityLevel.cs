using SEP490_FTCDHMM_API.Shared.Exceptions;

namespace SEP490_FTCDHMM_API.Domain.ValueObjects
{
    public record ActivityLevel(string Value, decimal Factor)
    {
        public static readonly ActivityLevel Sedentary = new("SENDENTARY", 1.2m);
        public static readonly ActivityLevel Light = new("LIGHT", 1.375m);
        public static readonly ActivityLevel Moderate = new("MODERATE", 1.55m);
        public static readonly ActivityLevel Active = new("ACTIVE", 1.725m);
        public static readonly ActivityLevel VeryActive = new("VERYACTIVE", 1.9m);

        public static ActivityLevel From(string value)
        {
            return value.Trim().ToUpperInvariant() switch
            {
                "SENDENTARY" => Sedentary,
                "LIGHT" => Light,
                "MODERATE" => Moderate,
                "ACTIVE" => Active,
                "VERYACTIVE" => VeryActive,
                _ => throw new AppException(AppResponseCode.INVALID_INPUT)
            };
        }
    }
}
