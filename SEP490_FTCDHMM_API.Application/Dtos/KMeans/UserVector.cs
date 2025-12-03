namespace SEP490_FTCDHMM_API.Application.Dtos.KMeans
{
    public class UserVector
    {
        public Guid UserId { get; set; }
        public double Tdee { get; set; }

        public double CarbPct { get; set; }
        public double ProteinPct { get; set; }
        public double FatPct { get; set; }

    }
}

