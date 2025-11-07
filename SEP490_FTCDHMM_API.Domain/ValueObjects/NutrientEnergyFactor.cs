namespace SEP490_FTCDHMM_API.Domain.ValueObjects
{
    public static class NutrientEnergyFactor
    {
        public static decimal KcalPerGram(string nutrientName)
            => nutrientName.Trim().ToUpperInvariant() switch
            {
                "PROTEIN" => 4m,
                "FAT" => 9m,
                "CARBONHYDRATE" => 4m,
                _ => 0m
            };
    }
}
