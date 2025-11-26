using Microsoft.EntityFrameworkCore;
using SEP490_FTCDHMM_API.Application.Dtos.KMeans;
using SEP490_FTCDHMM_API.Application.Interfaces.Persistence;
using SEP490_FTCDHMM_API.Application.Services.Interfaces;
using SEP490_FTCDHMM_API.Domain.Entities;
using SEP490_FTCDHMM_API.Domain.ValueObjects;

namespace SEP490_FTCDHMM_API.Application.Services.Implementations
{
    public class UserVectorBuilder : IUserVectorBuilder
    {
        private readonly IUserRepository _userRepo;
        private readonly IUserHealthMetricRepository _metricRepo;

        public UserVectorBuilder(
            IUserRepository userRepo,
            IUserHealthMetricRepository metricRepo)
        {
            _userRepo = userRepo;
            _metricRepo = metricRepo;
        }

        public async Task<List<UserVector>> BuildAllAsync()
        {
            // 1) Lấy toàn bộ user + các navigation cần cho interaction + health goal
            var users = await _userRepo.GetAllAsync(
                include: q => q
                    .Include(u => u.ViewedRecipes)
                    .Include(u => u.FavoriteRecipes)
                    .Include(u => u.SaveRecipes)
                    .Include(u => u.HealthGoals!)
                        .ThenInclude(uh => uh.HealthGoal)
                            .ThenInclude(h => h.Targets)
                                .ThenInclude(t => t.Nutrient)
            );

            // 2) Lấy toàn bộ UserHealthMetric, group theo UserId, lấy bản mới nhất
            var allMetrics = await _metricRepo.GetAllAsync();
            var latestMetricLookup = allMetrics
                .GroupBy(m => m.UserId)
                .ToDictionary(
                    g => g.Key,
                    g => g.OrderByDescending(x => x.RecordedAt).FirstOrDefault()
                );

            var result = new List<UserVector>();

            foreach (var u in users)
            {
                // --- 2.1. TDEE ---
                double tdee = 0;
                if (latestMetricLookup.TryGetValue(u.Id, out var latestMetric) && latestMetric != null)
                {
                    tdee = (double)latestMetric.TDEE;
                }

                // --- 2.2. Macro từ HealthGoal (EnergyPercent) ---
                double carbPct = 0, proteinPct = 0, fatPct = 0;

                // Lấy HealthGoal mới nhất của user
                var latestUserHealthGoal = u.HealthGoals
                    .OrderByDescending(hg => hg.CreatedAtUtc)
                    .FirstOrDefault();

                var healthGoal = latestUserHealthGoal?.HealthGoal;

                if (healthGoal != null)
                {
                    ExtractMacroEnergyPercent(healthGoal, out carbPct, out proteinPct, out fatPct);
                }
                // --- 2.3. Interaction scores ---
                var viewScore = u.ViewedRecipes.Count;
                var favoriteScore = u.FavoriteRecipes.Count;
                var saveScore = u.SaveRecipes.Count;

                // --- 2.4. Map sang UserVector ---
                result.Add(new UserVector
                {
                    UserId = u.Id,
                    Tdee = tdee,
                    CarbPct = carbPct,
                    ProteinPct = proteinPct,
                    FatPct = fatPct,
                    ViewScore = viewScore,
                    FavoriteScore = favoriteScore,
                    SaveScore = saveScore
                });
            }

            return result;
        }

        // Helper: trích macro EnergyPercent từ HealthGoal
        private static void ExtractMacroEnergyPercent(
            HealthGoal healthGoal,
            out double carbPct,
            out double proteinPct,
            out double fatPct)
        {
            carbPct = proteinPct = fatPct = 0;

            foreach (var t in healthGoal.Targets)
            {
                if (t.TargetType != NutrientTargetType.EnergyPercent)
                    continue;

                var nutrientName = t.Nutrient.Name.Trim().ToLowerInvariant();

                // Tùy bạn naming, có thể là "Carb", "Carbohydrate", "Carbohydrates"...
                if (nutrientName.StartsWith("carb"))
                {
                    carbPct = ToRatio(t.MinEnergyPct, t.MaxEnergyPct);
                }
                else if (nutrientName.StartsWith("protein"))
                {
                    proteinPct = ToRatio(t.MinEnergyPct, t.MaxEnergyPct);
                }
                else if (nutrientName.StartsWith("fat") || nutrientName.StartsWith("lipid"))
                {
                    fatPct = ToRatio(t.MinEnergyPct, t.MaxEnergyPct);
                }
            }
        }

        // MinEnergyPct/MaxEnergyPct là % (ví dụ 20–30)
        // Ta convert về tỷ lệ 0–1 (ví dụ 0.25)
        private static double ToRatio(decimal? minPct, decimal? maxPct)
        {
            // Lấy trung bình, rồi / 100
            if (minPct.HasValue && maxPct.HasValue && minPct < maxPct)
            {
                var avg = (double)((minPct.Value + maxPct.Value) / 2m);
                return avg / 100.0;
            }

            if (minPct.HasValue)
                return (double)minPct.Value / 100.0;

            if (maxPct.HasValue)
                return (double)maxPct.Value / 100.0;

            return 0;
        }
    }
}