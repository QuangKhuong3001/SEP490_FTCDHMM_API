using SEP490_FTCDHMM_API.Application.Dtos.KMeans;
using SEP490_FTCDHMM_API.Application.Services.Interfaces;

namespace SEP490_FTCDHMM_API.Application.Services.Implementations
{
    public class KMeansService : IKMeansService
    {
        private readonly Random _rand = new();

        public ClusterOutput Compute(List<UserVector> users, int k)
        {
            if (users.Count == 0)
                throw new ArgumentException("User vector list is empty.");

            if (k <= 0 || k > users.Count)
                throw new ArgumentException("Invalid cluster number K.");

            // 1) Chuyển UserVector → double[] và random chọn K tâm cụm ban đầu
            var userVectors = users.Select(ToArray).ToList();

            var centroids = userVectors
                .OrderBy(x => _rand.Next())
                .Take(k)
                .Select(v => (double[])v.Clone())
                .ToList();

            bool changed = true;
            var assignments = new Dictionary<Guid, int>();

            // 2) Loop KMeans cho tới khi phân cụm không đổi
            while (changed)
            {
                changed = false;

                // Gán từng user vào cụm gần nhất
                for (int idx = 0; idx < users.Count; idx++)
                {
                    var u = users[idx];
                    var vec = userVectors[idx];

                    int nearestCluster = GetNearestCluster(vec, centroids);

                    if (!assignments.ContainsKey(u.UserId) ||
                        assignments[u.UserId] != nearestCluster)
                    {
                        assignments[u.UserId] = nearestCluster;
                        changed = true;
                    }
                }

                // 3) Cập nhật lại tâm cụm
                for (int c = 0; c < k; c++)
                {
                    var indices = users
                        .Select((u, idx) => new { u.UserId, idx })
                        .Where(x => assignments[x.UserId] == c)
                        .Select(x => x.idx)
                        .ToList();

                    if (!indices.Any())
                        continue;

                    var newCenter = new double[userVectors[0].Length];

                    foreach (var i in indices)
                    {
                        var v = userVectors[i];
                        for (int d = 0; d < newCenter.Length; d++)
                        {
                            newCenter[d] += v[d];
                        }
                    }

                    for (int d = 0; d < newCenter.Length; d++)
                    {
                        newCenter[d] /= indices.Count;
                    }

                    centroids[c] = newCenter;
                }
            }

            // 4) Build kết quả
            return new ClusterOutput
            {
                Assignments = assignments
                    .Select(a => new UserClusterResult
                    {
                        UserId = a.Key,
                        ClusterId = a.Value
                    })
                    .ToList(),
                Centroids = centroids
                    .Select((c, i) => new { Index = i, Center = c })
                    .ToDictionary(x => x.Index, x => x.Center)
            };
        }

        // Chuyển UserVector → mảng double[]
        private double[] ToArray(UserVector u)
        {
            return new[]
            {
                u.Tdee,
                u.CarbPct,
                u.ProteinPct,
                u.FatPct,
                u.ViewScore,
                u.FavoriteScore,
                u.SaveScore
            };
        }

        // Tìm cụm gần nhất cho vector v
        private int GetNearestCluster(double[] v, List<double[]> centroids)
        {
            double minDistance = double.MaxValue;
            int nearestIndex = 0;

            for (int i = 0; i < centroids.Count; i++)
            {
                var c = centroids[i];

                double distance = 0;
                for (int d = 0; d < c.Length; d++)
                {
                    var diff = v[d] - c[d];
                    distance += diff * diff; // dùng squared distance, bỏ sqrt
                }

                if (distance < minDistance)
                {
                    minDistance = distance;
                    nearestIndex = i;
                }
            }

            return nearestIndex;
        }
    }
}
