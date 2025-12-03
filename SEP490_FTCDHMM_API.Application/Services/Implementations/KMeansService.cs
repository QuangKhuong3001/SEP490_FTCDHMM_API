using SEP490_FTCDHMM_API.Application.Dtos.KMeans;
using SEP490_FTCDHMM_API.Application.Services.Interfaces;
using SEP490_FTCDHMM_API.Shared.Exceptions;

namespace SEP490_FTCDHMM_API.Application.Services.Implementations
{
    public class KMeansService : IKMeansService
    {
        private readonly Random _rand = new();

        public ClusterOutput Compute(List<UserVector> users, int k)
        {
            if (users.Count == 0)
                throw new AppException(AppResponseCode.NOT_FOUND, "Danh sách người dùng trống");

            if (k <= 0 || k > users.Count)
                throw new AppException(AppResponseCode.INVALID_ACTION, "Số lượng nhóm phải lớn hơn 0 và ít hơn số người dùng");

            var userVectors = users.Select(ToArray).ToList();

            var centroids = userVectors
                .OrderBy(x => _rand.Next())
                .Take(k)
                .Select(v => (double[])v.Clone())
                .ToList();

            bool changed = true;
            var assignments = new Dictionary<Guid, int>();

            while (changed)
            {
                changed = false;

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

        private double[] ToArray(UserVector u)
        {
            return new[]
            {
                u.Tdee,
                u.CarbPct,
                u.ProteinPct,
                u.FatPct
            };
        }

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
                    distance += diff * diff;
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
