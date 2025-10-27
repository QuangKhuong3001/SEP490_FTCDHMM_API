using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SEP490_FTCDHMM_API.Application.Dtos.HealthGoalDtos;
using SEP490_FTCDHMM_API.Application.Interfaces.Persistence;
using SEP490_FTCDHMM_API.Application.Services.Interfaces;
using SEP490_FTCDHMM_API.Domain.Entities;
using SEP490_FTCDHMM_API.Domain.ValueObjects;
using SEP490_FTCDHMM_API.Shared.Exceptions;

namespace SEP490_FTCDHMM_API.Application.Services.Implementations
{
    public class HealthGoalService : IHealthGoalService
    {
        private readonly IHealthGoalRepository _healthGoalRepository;
        private readonly INutrientRepository _nutrientRepository;
        private readonly IMapper _mapper;

        public HealthGoalService(IHealthGoalRepository healthGoalRepository, INutrientRepository nutrientRepository, IMapper mapper)
        {
            _healthGoalRepository = healthGoalRepository;
            _nutrientRepository = nutrientRepository;
            _mapper = mapper;
        }

        public async Task CreateAsync(CreateHealthGoalRequest request)
        {
            var nutrientIds = request.Targets.Select(n => n.NutrientId).ToList();

            var exist = await _nutrientRepository.IdsExistAsync(nutrientIds);

            if (!exist)
                throw new AppException(AppResponseCode.NOT_FOUND);

            var duplicateIds = request.Targets
                .GroupBy(t => t.NutrientId)
                .Where(g => g.Count() > 1)
                .Select(g => g.Key)
                .ToList();

            if (duplicateIds.Any())
            {
                throw new AppException(AppResponseCode.INVALID_ACTION);
            }

            foreach (var nutrient in request.Targets)
            {
                if (nutrient.MaxValue <= nutrient.MinValue)
                    throw new AppException(AppResponseCode.INVALID_ACTION);
            }

            var goal = new HealthGoal
            {
                Name = request.Name,
                Description = request.Description,
                Targets = request.Targets.Select(t => new HealthGoalTarget
                {
                    NutrientId = t.NutrientId,
                    TargetType = NutrientTargetType.From(t.TargetType),
                    MinValue = t.MinValue,
                    MedianValue = t.MedianValue,
                    MaxValue = t.MaxValue,
                    MinEnergyPct = t.MinEnergyPct,
                    MedianEnergyPct = t.MedianEnergyPct,
                    MaxEnergyPct = t.MaxEnergyPct,
                    Weight = t.Weight
                }).ToList()
            };

            await _healthGoalRepository.AddAsync(goal);
        }

        public async Task UpdateAsync(Guid id, UpdateHealthGoalRequest request)
        {
            var healthGoal = await _healthGoalRepository.GetByIdAsync(id, include: i => i.Include(h => h.Targets));

            if (healthGoal == null)
                throw new AppException(AppResponseCode.NOT_FOUND);

            var nutrientIds = request.Targets.Select(n => n.NutrientId).ToList();

            var exist = await _nutrientRepository.IdsExistAsync(nutrientIds);

            if (!exist)
                throw new AppException(AppResponseCode.NOT_FOUND);

            var duplicateIds = request.Targets
                .GroupBy(t => t.NutrientId)
                .Where(g => g.Count() > 1)
                .Select(g => g.Key)
                .ToList();

            if (duplicateIds.Any())
            {
                throw new AppException(AppResponseCode.INVALID_ACTION);
            }

            foreach (var nutrient in request.Targets)
            {
                if (nutrient.MaxValue <= nutrient.MinValue)
                    throw new AppException(AppResponseCode.INVALID_ACTION);
            }

            healthGoal.Targets.Clear();

            healthGoal.Description = request.Description;
            healthGoal.Targets = request.Targets.Select(t => new HealthGoalTarget
            {
                NutrientId = t.NutrientId,
                TargetType = NutrientTargetType.From(t.TargetType),
                MinValue = t.MinValue,
                MedianValue = t.MedianValue,
                MaxValue = t.MaxValue,
                MinEnergyPct = t.MinEnergyPct,
                MedianEnergyPct = t.MedianEnergyPct,
                MaxEnergyPct = t.MaxEnergyPct,
                Weight = t.Weight
            }).ToList();

            await _healthGoalRepository.UpdateAsync(healthGoal);
        }

        public async Task<IReadOnlyList<HealthGoalResponse>> GetAllAsync()
        {
            var goals = await _healthGoalRepository.GetAllAsync(
                include: q => q.Include(g => g.Targets).ThenInclude(t => t.Nutrient)

            );

            var result = _mapper.Map<List<HealthGoalResponse>>(goals);

            return result;
        }

        public async Task<HealthGoalResponse> GetByIdAsync(Guid id)
        {
            var goal = await _healthGoalRepository.GetByIdAsync(id,
                include: q => q.Include(g => g.Targets).ThenInclude(t => t.Nutrient));

            var result = _mapper.Map<HealthGoalResponse>(goal);
            return result;

        }

        public async Task DeleteAsync(Guid id)
        {
            var goal = await _healthGoalRepository.GetByIdAsync(id);
            if (goal == null)
                throw new AppException(AppResponseCode.NOT_FOUND);

            await _healthGoalRepository.DeleteAsync(goal);
        }
    }
}
