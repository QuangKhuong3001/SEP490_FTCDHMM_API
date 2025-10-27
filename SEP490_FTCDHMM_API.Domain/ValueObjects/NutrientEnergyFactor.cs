namespace SEP490_FTCDHMM_API.Domain.ValueObjects
{
    public static class NutrientEnergyFactor
    {
        public static decimal KcalPerGram(string nutrientName)
            => nutrientName.ToLower() switch
            {
                "protein" => 4m,
                "fat" => 9m,
                "carbohydrate" => 4m,
                "alcohol" => 7m,
                _ => 0m
            };
    }
}
