using SEP490_FTCDHMM_API.Shared.Exceptions;

namespace SEP490_FTCDHMM_API.Domain.ValueObjects
{
    public record DifficultyValue(string Value)
    {
        public static readonly DifficultyValue Hard = new("HARD");
        public static readonly DifficultyValue Medium = new("MEDIUM");
        public static readonly DifficultyValue Easy = new("EASY");

        public static DifficultyValue From(string value)
        {
            return value.Trim().ToUpperInvariant() switch
            {
                "EASY" => Easy,
                "MEDIUM" => Medium,
                "HARD" => Hard,
                _ => throw new AppException(AppResponseCode.INVALID_ACTION)
            };
        }
        public override string ToString() => Value;
    }
}
