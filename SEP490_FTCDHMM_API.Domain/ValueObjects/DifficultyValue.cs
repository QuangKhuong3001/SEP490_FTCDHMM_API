using SEP490_FTCDHMM_API.Shared.Exceptions;

namespace SEP490_FTCDHMM_API.Domain.ValueObjects
{
    public record DifficultyValue(string Value)
    {
        public static readonly DifficultyValue Hard = new("Hard");
        public static readonly DifficultyValue Medium = new("Medium");
        public static readonly DifficultyValue Easy = new("Easy");

        public static DifficultyValue From(string value)
        {
            return value switch
            {
                "Easy" => Easy,
                "Medium" => Medium,
                "Hard" => Hard,
                _ => throw new AppException(AppResponseCode.INVALID_INPUT)
            };
        }
        public override string ToString() => Value;
    }
}
