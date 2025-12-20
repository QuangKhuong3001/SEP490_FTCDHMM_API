namespace SEP490_FTCDHMM_API.Application.Dtos.KMeans
{
    public class ClusterOutput
    {
        public List<UserClusterResult> Assignments { get; set; } = new();
        public Dictionary<int, double[]> Centroids { get; set; } = new();
        public double Inertia { get; set; }
    }
}
