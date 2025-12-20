using SEP490_FTCDHMM_API.Application.Dtos.KMeans;
using SEP490_FTCDHMM_API.Application.Services.Interfaces.ClusterInterfaces;
using SEP490_FTCDHMM_API.Shared.Exceptions;

namespace SEP490_FTCDHMM_API.Application.Services.Implementations.ClusterImplementations
{
    public class KMeansService : IKMeansService
    {
        private readonly Random _rand = new(42);

        public double CalculateSilhouette(List<UserVector> users, List<UserClusterResult> assignments)
        {
            var vectors = Standardize(users.Select(ToArray).ToList());
            var labelMap = assignments.ToDictionary(a => a.UserId, a => a.ClusterId);

            double total = 0;
            int n = users.Count;

            for (int i = 0; i < n; i++)
            {
                var u = users[i];
                var v = vectors[i];
                int cluster = labelMap[u.UserId];

                var same = new List<double>();
                var other = new Dictionary<int, List<double>>();

                for (int j = 0; j < n; j++)
                {
                    if (i == j) continue;

                    double dist = Distance(v, vectors[j]);
                    int otherCluster = labelMap[users[j].UserId];

                    if (otherCluster == cluster)
                    {
                        same.Add(dist);
                    }
                    else
                    {
                        if (!other.ContainsKey(otherCluster))
                            other[otherCluster] = new List<double>();

                        other[otherCluster].Add(dist);
                    }
                }

                double s;

                if (!same.Any() || !other.Any())
                {
                    s = 0;
                }
                else
                {
                    double a = same.Average();
                    double b = other.Values.Min(x => x.Average());
                    s = (b - a) / Math.Max(a, b);
                }

                total += s;
            }

            return total / n;
        }

        private double Distance(double[] a, double[] b)
        {
            double sum = 0;
            for (int i = 0; i < a.Length; i++)
            {
                var diff = a[i] - b[i];
                sum += diff * diff;
            }
            return Math.Sqrt(sum);
        }


        public ClusterOutput Compute(List<UserVector> users, int k)
        {
            if (users.Count == 0)
                throw new AppException(AppResponseCode.NOT_FOUND, "Danh sách người dùng trống");

            if (k <= 0 || k > users.Count)
                throw new AppException(AppResponseCode.INVALID_ACTION, "Số lượng nhóm phải lớn hơn 0 và ít hơn số người dùng");

            var rawVectors = users.Select(ToArray).ToList();
            var scaledVectors = Standardize(rawVectors);

            var centroidsScaled = scaledVectors
                .OrderBy(x => _rand.Next())
                .Take(k)
                .Select(v => (double[])v.Clone())
                .ToList();

            bool changed = true;
            var assignments = new Dictionary<Guid, int>();

            while (changed)
            {
                changed = false;

                for (int i = 0; i < users.Count; i++)
                {
                    var nearest = GetNearestCluster(scaledVectors[i], centroidsScaled);

                    if (!assignments.ContainsKey(users[i].UserId) ||
                        assignments[users[i].UserId] != nearest)
                    {
                        assignments[users[i].UserId] = nearest;
                        changed = true;
                    }
                }

                for (int c = 0; c < k; c++)
                {
                    var indices = assignments
                        .Where(x => x.Value == c)
                        .Select(x => users.FindIndex(u => u.UserId == x.Key))
                        .Where(i => i >= 0)
                        .ToList();

                    if (!indices.Any())
                        continue;

                    var newCenter = new double[scaledVectors[0].Length];

                    foreach (var i in indices)
                    {
                        for (int d = 0; d < newCenter.Length; d++)
                            newCenter[d] += scaledVectors[i][d];
                    }

                    for (int d = 0; d < newCenter.Length; d++)
                        newCenter[d] /= indices.Count;

                    centroidsScaled[c] = newCenter;
                }
            }

            var centroidsRaw = new Dictionary<int, double[]>();

            for (int c = 0; c < k; c++)
            {
                var indices = assignments
                    .Where(x => x.Value == c)
                    .Select(x => users.FindIndex(u => u.UserId == x.Key))
                    .Where(i => i >= 0)
                    .ToList();

                if (!indices.Any())
                    continue;

                var center = new double[rawVectors[0].Length];

                foreach (var i in indices)
                {
                    for (int d = 0; d < center.Length; d++)
                        center[d] += rawVectors[i][d];
                }

                for (int d = 0; d < center.Length; d++)
                    center[d] /= indices.Count;

                centroidsRaw[c] = center;
            }

            double inertia = 0;

            foreach (var a in assignments)
            {
                var idx = users.FindIndex(u => u.UserId == a.Key);
                var v = scaledVectors[idx];
                var c = centroidsScaled[a.Value];

                for (int d = 0; d < v.Length; d++)
                {
                    var diff = v[d] - c[d];
                    inertia += diff * diff;
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
                Centroids = centroidsRaw,
                Inertia = inertia
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

        private List<double[]> Standardize(List<double[]> data)
        {
            int rows = data.Count;
            int cols = data[0].Length;

            var means = new double[cols];
            var stds = new double[cols];

            for (int j = 0; j < cols; j++)
            {
                means[j] = data.Average(x => x[j]);
                stds[j] = Math.Sqrt(data.Average(x => Math.Pow(x[j] - means[j], 2)));
            }

            var result = new List<double[]>();

            foreach (var row in data)
            {
                var scaled = new double[cols];
                for (int j = 0; j < cols; j++)
                    scaled[j] = stds[j] == 0 ? 0 : (row[j] - means[j]) / stds[j];

                result.Add(scaled);
            }

            return result;
        }
    }
}
