﻿using SEP490_FTCDHMM_API.Domain.ValueObjects;

namespace SEP490_FTCDHMM_API.Domain.Entities
{
    public class HealthGoalTarget
    {
        public Guid Id { get; set; }

        public Guid HealthGoalId { get; set; }
        public HealthGoal HealthGoal { get; set; } = null!;

        public Guid NutrientId { get; set; }
        public Nutrient Nutrient { get; set; } = null!;

        public NutrientTargetType TargetType { get; set; } = NutrientTargetType.Absolute;

        public decimal? MinValue { get; set; }
        public decimal? MaxValue { get; set; }

        public decimal? MinEnergyPct { get; set; }
        public decimal? MaxEnergyPct { get; set; }

        public decimal Weight { get; set; } = 1m;
    }
}
