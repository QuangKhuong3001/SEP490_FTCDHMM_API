namespace SEP490_FTCDHMM_API.Domain.Services
{
    public static class BmrCalculator
    {
        public static decimal Calculate(
            decimal weightKg,
            decimal heightCm,
            int age,
            string gender,
            decimal? muscleMassKg = null,
            decimal? bodyFatPercent = null)
        {
            //Cunningham
            if (muscleMassKg.HasValue || bodyFatPercent.HasValue)
            {
                decimal lbm;
                if (muscleMassKg.HasValue)
                    lbm = muscleMassKg.Value / 0.9m;
                else
                    lbm = weightKg * (1 - (bodyFatPercent!.Value / 100));

                return 500 + (22 * lbm);
            }

            //Mifflin–St Jeor
            int genderFactor = gender.ToUpperInvariant() == "MALE" ? 5 : -161;
            return (10 * weightKg) + (6.25m * heightCm) - (5 * age) + genderFactor;
        }
    }
}
