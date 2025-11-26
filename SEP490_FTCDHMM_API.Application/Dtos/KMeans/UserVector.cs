namespace SEP490_FTCDHMM_API.Application.Dtos.KMeans
{
    public class UserVector
    {
        public Guid UserId { get; set; }

        // 1. TDEE
        public double Tdee { get; set; }

        // 2. HealthGoal (macro % năng lượng)
        public double CarbPct { get; set; }
        public double ProteinPct { get; set; }
        public double FatPct { get; set; }

        // 3. Interaction
        public double ViewScore { get; set; }
        public double FavoriteScore { get; set; }
        public double SaveScore { get; set; }
    }
}

