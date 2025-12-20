namespace SEP490_FTCDHMM_API.Application.Dtos.KMeans
{
    public class KMeansEvaluationResult
    {
        public Dictionary<int, double> Elbow { get; set; } = new();
        public Dictionary<int, double> Silhouette { get; set; } = new();
        public int BestK { get; set; }
    }
}
