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

        public UserVectorBuilder(
            IUserRepository userRepo)
        {
            _userRepo = userRepo;
        }

        public async Task<List<UserVector>> BuildAllAsync()
        {
            var users = await _userRepo.GetAllAsync(
                include: q => q
                    .Include(u => u.UserHealthGoals!)
                        .ThenInclude(uh => uh.HealthGoal!)
                            .ThenInclude(h => h.Targets)
                                .ThenInclude(t => t.Nutrient)

                    .Include(u => u.UserHealthGoals!)
                        .ThenInclude(uh => uh.CustomHealthGoal!)
                            .ThenInclude(ch => ch.Targets)
                                .ThenInclude(t => t.Nutrient)

                    .Include(u => u.HealthMetrics)
            );

            var result = new List<UserVector>();

            foreach (var u in users)
            {
                var metric = u.HealthMetrics
                    .OrderByDescending(m => m.RecordedAt)
                    .FirstOrDefault();

                if (metric == null)
                    continue;

                var activeGoal = u.UserHealthGoals
                    .Where(hg => hg.ExpiredAtUtc == null || hg.ExpiredAtUtc > DateTime.UtcNow)
                    .OrderByDescending(hg => hg.StartedAtUtc)
                    .FirstOrDefault();

                if (activeGoal == null)
                    continue;

                double carbPct = 0, proteinPct = 0, fatPct = 0;

                if (activeGoal.Type == HealthGoalType.SYSTEM && activeGoal.HealthGoal != null)
                {
                    ExtractMacroEnergyPercent(activeGoal.HealthGoal.Targets,
                        out carbPct, out proteinPct, out fatPct);
                }
                else if (activeGoal.Type == HealthGoalType.CUSTOM && activeGoal.CustomHealthGoal != null)
                {
                    ExtractMacroEnergyPercent(activeGoal.CustomHealthGoal.Targets,
                        out carbPct, out proteinPct, out fatPct);
                }
                else
                {
                    continue;
                }

                result.Add(new UserVector
                {
                    UserId = u.Id,
                    Tdee = (double)metric.TDEE,
                    CarbPct = carbPct,
                    ProteinPct = proteinPct,
                    FatPct = fatPct
                });
            }

            return result;
        }

        private static void ExtractMacroEnergyPercent(
            IEnumerable<HealthGoalTarget> targets,
            out double carbPct,
            out double proteinPct,
            out double fatPct)
        {
            carbPct = proteinPct = fatPct = 0;

            foreach (var t in targets)
            {
                if (t.TargetType != NutrientTargetType.EnergyPercent)
                    continue;

                var name = t.Nutrient.Name.Trim().ToLowerInvariant();

                if (name.StartsWith("carb"))
                    carbPct = ToRatio(t.MinEnergyPct, t.MaxEnergyPct);
                else if (name.StartsWith("protein"))
                    proteinPct = ToRatio(t.MinEnergyPct, t.MaxEnergyPct);
                else if (name.StartsWith("fat") || name.StartsWith("lipid"))
                    fatPct = ToRatio(t.MinEnergyPct, t.MaxEnergyPct);
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