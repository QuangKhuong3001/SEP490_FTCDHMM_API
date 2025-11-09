using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SEP490_FTCDHMM_API.Application.Dtos.CustomHealthGoalDtos;
using SEP490_FTCDHMM_API.Application.Dtos.HealthGoalDtos;
using SEP490_FTCDHMM_API.Application.Interfaces.Persistence;
using SEP490_FTCDHMM_API.Application.Services.Interfaces;
using SEP490_FTCDHMM_API.Domain.Entities;
using SEP490_FTCDHMM_API.Domain.ValueObjects;
using SEP490_FTCDHMM_API.Shared.Exceptions;

namespace SEP490_FTCDHMM_API.Application.Services.Implementations
{
    public class CustomHealthGoalService : ICustomHealthGoalService
    {
        private readonly ICustomHealthGoalRepository _customHealthGoalRepository;
        private readonly IMapper _mapper;
        private readonly INutrientRepository _nutrientRepository;

        public CustomHealthGoalService(ICustomHealthGoalRepository customHealthGoalRepository, IMapper mapper, INutrientRepository nutrientRepository)
        {
            _customHealthGoalRepository = customHealthGoalRepository;
            _mapper = mapper;
            _nutrientRepository = nutrientRepository;
        }

        public async Task CreateAsync(Guid userId, CreateCustomHealthGoalRequest request)
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

            var goal = new CustomHealthGoal
            {
                UserId = userId,
                Name = request.Name,
                Description = request.Description,
                Targets = request.Targets.Select(t => new CustomHealthGoalTarget
                {
                    NutrientId = t.NutrientId,
                    TargetType = NutrientTargetType.From(t.TargetType),
                    MinValue = t.MinValue,
                    MaxValue = t.MaxValue,
                    MinEnergyPct = t.MinEnergyPct,
                    MaxEnergyPct = t.MaxEnergyPct,
                    Weight = t.Weight
                }).ToList()
            };

            await _customHealthGoalRepository.AddAsync(goal);
        }

        public async Task<IEnumerable<HealthGoalResponse>> GetMyGoalsAsync(Guid userId)
        {
            var goals = await _customHealthGoalRepository.GetAllAsync(
                predicate: g => g.UserId == userId,
                include: q => q.Include(g => g.Targets)
                                    .ThenInclude(t => t.Nutrient));
            return _mapper.Map<IEnumerable<HealthGoalResponse>>(goals);
        }

        public async Task UpdateAsync(Guid userId, Guid id, UpdateCustomHealthGoalRequest request)
        {
            var healthGoal = await _customHealthGoalRepository.GetByIdAsync(id,
                include: i => i.Include(h => h.Targets));

            if (healthGoal == null)
                throw new AppException(AppResponseCode.NOT_FOUND);

            if (healthGoal.UserId != userId)
                throw new AppException(AppResponseCode.FORBIDDEN);

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

            healthGoal.Name = request.Name;
            healthGoal.Description = request.Description;
            healthGoal.Targets = request.Targets.Select(t => new CustomHealthGoalTarget
            {
                NutrientId = t.NutrientId,
                TargetType = NutrientTargetType.From(t.TargetType),
                MinValue = t.MinValue,
                MaxValue = t.MaxValue,
                MinEnergyPct = t.MinEnergyPct,
                MaxEnergyPct = t.MaxEnergyPct,
                Weight = t.Weight
            }).ToList();

            await _customHealthGoalRepository.UpdateAsync(healthGoal);
        }

        public async Task<HealthGoalResponse> GetByIdAsync(Guid userId, Guid id)
        {
            var goal = await _customHealthGoalRepository.GetByIdAsync(id,
                include: q => q.Include(g => g.Targets).ThenInclude(t => t.Nutrient));

            if (goal == null)
                throw new AppException(AppResponseCode.NOT_FOUND);

            if (goal.UserId != userId)
                throw new AppException(AppResponseCode.FORBIDDEN);

            var result = _mapper.Map<HealthGoalResponse>(goal);
            return result;
        }

        public async Task DeleteAsync(Guid userId, Guid id)
        {
            var goal = await _customHealthGoalRepository.GetByIdAsync(id);
            if (goal == null)
                throw new AppException(AppResponseCode.NOT_FOUND);

            if (goal.UserId != userId)
                throw new AppException(AppResponseCode.FORBIDDEN);

            await _customHealthGoalRepository.DeleteAsync(goal);
        }

        public async Task ActiveAsync(Guid userId, Guid id)
        {
            var goal = await _customHealthGoalRepository.GetByIdAsync(id);
            if (goal == null)
                throw new AppException(AppResponseCode.NOT_FOUND);

            if (goal.UserId != userId)
                throw new AppException(AppResponseCode.FORBIDDEN);

            if (goal.IsActive)
                throw new AppException(AppResponseCode.INVALID_ACTION);

            goal.IsActive = true;

            await _customHealthGoalRepository.UpdateAsync(goal);
        }
        public async Task DeActiveAsync(Guid userId, Guid id)
        {
            var goal = await _customHealthGoalRepository.GetByIdAsync(id);
            if (goal == null)
                throw new AppException(AppResponseCode.NOT_FOUND);

            if (goal.UserId != userId)
                throw new AppException(AppResponseCode.FORBIDDEN);

            if (!goal.IsActive)
                throw new AppException(AppResponseCode.INVALID_ACTION);

            goal.IsActive = false;

            await _customHealthGoalRepository.UpdateAsync(goal);
        }

    }

}
