using SEP490_FTCDHMM_API.Shared.Exceptions;

namespace SEP490_FTCDHMM_API.Domain.ValueObjects
{
    public record RestrictionType(string Value)
    {
        public static readonly RestrictionType Allergy = new("ALLERGY");
        public static readonly RestrictionType Dislike = new("DISLIKE");
        public static readonly RestrictionType TemporaryAvoid = new("TEMPORARYAVOID");

        public override string ToString() => Value;

        public static RestrictionType From(string value)
        {
            return value.Trim().ToUpperInvariant() switch
            {
                "ALLERGY" => Allergy,
                "DISLIKE" => Dislike,
                "TEMPORARYAVOID" => TemporaryAvoid,
                _ => throw new AppException(AppResponseCode.INVALID_ACTION)
            };
        }
    }
}
