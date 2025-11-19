using SEP490_FTCDHMM_API.Shared.Exceptions;

namespace SEP490_FTCDHMM_API.Domain.ValueObjects
{
    public record Gender(string Value)
    {
        public static readonly Gender Male = new("MALE");
        public static readonly Gender Female = new("FEMALE");

        public override string ToString() => Value;

        public static Gender From(string value)
        {
            return value.Trim().ToUpperInvariant() switch
            {
                "MALE" => Male,
                "FEMALE" => Female,
                _ => throw new AppException(AppResponseCode.INVALID_ACTION, "Giới tính không hợp lệ.")
            };
        }
    }
}
