using SEP490_FTCDHMM_API.Shared.Exceptions;

namespace SEP490_FTCDHMM_API.Domain.ValueObjects
{
    public record NutrientTargetType(string Value)
    {
        public static readonly NutrientTargetType Absolute = new("ABSOLUTE");
        public static readonly NutrientTargetType EnergyPercent = new("ENERGYPERCENT");

        public override string ToString() => Value;

        public static NutrientTargetType From(string value) =>
            value.Trim().ToUpperInvariant() switch
            {
                "ABSOLUTE" => Absolute,
                "ENERGYPERCENT" => EnergyPercent,
                _ => throw new AppException(AppResponseCode.INVALID_ACTION)
            };
    }
}