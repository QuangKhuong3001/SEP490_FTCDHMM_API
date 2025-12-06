using Microsoft.EntityFrameworkCore;
using SEP490_FTCDHMM_API.Application.Dtos.KMeans;
using SEP490_FTCDHMM_API.Application.Interfaces.Persistence;
using SEP490_FTCDHMM_API.Application.Services.Interfaces.ClusterInterfaces;
using SEP490_FTCDHMM_API.Domain.Entities;
using SEP490_FTCDHMM_API.Domain.ValueObjects;

namespace SEP490_FTCDHMM_API.Application.Services.Implementations.ClusterImplementations
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
            var users = await _userRepo.GetAllAsync(
                include: q => q
                    .Include(u => u.HealthGoals!)
                        .ThenInclude(uh => uh.HealthGoal)
                            .ThenInclude(h => h!.Targets)
                                .ThenInclude(t => t.Nutrient)
                    .Include(u => u.HealthMetrics)
            );

            var result = new List<UserVector>();

            foreach (var u in users)
            {
                var activatingMetric = u.HealthMetrics.OrderByDescending(uh => uh.RecordedAt).FirstOrDefault();

                double tdee = 0;
                if (activatingMetric != null)
                {
                    tdee = (double)activatingMetric.TDEE;
                }

                double carbPct = 0, proteinPct = 0, fatPct = 0;

                var latestUserHealthGoal = u.HealthGoals
                    .OrderByDescending(hg => hg.StartedAtUtc)
                    .FirstOrDefault();

                var healthGoal = latestUserHealthGoal?.HealthGoal;

                if (healthGoal != null)
                {
                    ExtractMacroEnergyPercent(healthGoal, out carbPct, out proteinPct, out fatPct);
                }

                result.Add(new UserVector
                {
                    UserId = u.Id,
                    Tdee = tdee,
                    CarbPct = carbPct,
                    ProteinPct = proteinPct,
                    FatPct = fatPct
                });
            }

            return result;
        }

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

        private static double ToRatio(decimal? minPct, decimal? maxPct)
        {
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