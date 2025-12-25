using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SEP490_FTCDHMM_API.Application.Dtos.HealthGoalDtos;
using SEP490_FTCDHMM_API.Application.Dtos.NutrientDtos.NutrientTarget;
using SEP490_FTCDHMM_API.Application.Dtos.UserHealthGoalDtos;
using SEP490_FTCDHMM_API.Application.Interfaces.Persistence;
using SEP490_FTCDHMM_API.Application.Interfaces.SystemServices;
using SEP490_FTCDHMM_API.Application.Services.Interfaces.HealthGoalInterfaces;
using SEP490_FTCDHMM_API.Domain.Entities;
using SEP490_FTCDHMM_API.Domain.ValueObjects;
using SEP490_FTCDHMM_API.Shared.Exceptions;
using SEP490_FTCDHMM_API.Shared.Utils;

namespace SEP490_FTCDHMM_API.Application.Services.Implementations.HealthGoalImplementations
{
    public class HealthGoalService : IHealthGoalService
    {
        private readonly IHealthGoalRepository _healthGoalRepository;
        private readonly INutrientRepository _nutrientRepository;
        private readonly IMapper _mapper;
        private readonly ICustomHealthGoalRepository _customHealthGoalRepository;
        private readonly IHealthGoalTargetRepository _healthGoalTargetRepository;
        private readonly ICacheService _cache;

        public HealthGoalService(IHealthGoalRepository healthGoalRepository,
            INutrientRepository nutrientRepository,
            IHealthGoalTargetRepository healthGoalTargetRepository,
            IMapper mapper,
            ICacheService cache,
            ICustomHealthGoalRepository customHealthGoalRepository)
        {
            _healthGoalRepository = healthGoalRepository;
            _nutrientRepository = nutrientRepository;
            _cache = cache;
            _mapper = mapper;
            _healthGoalTargetRepository = healthGoalTargetRepository;
            _customHealthGoalRepository = customHealthGoalRepository;
        }

        public async Task CreateHealthGoalAsync(CreateHealthGoalRequest request)
        {
            var upperName = request.Name.ToUpperInvariant().CleanDuplicateSpace();

            var exist = await _healthGoalRepository.ExistsAsync(g => g.UpperName == upperName);

            if (exist)
                throw new AppException(AppResponseCode.EXISTS);

            var duplicateIds = request.Targets
                .GroupBy(t => t.NutrientId)
                .Where(g => g.Count() > 1)
                .Select(g => g.Key)
                .ToList();

            if (duplicateIds.Any())
            {
                throw new AppException(AppResponseCode.DUPLICATE, "Dinh dưỡng bị trùng lặp");
            }

            var totalPct = 0m;
            foreach (var nutrient in request.Targets)
            {
                await ValidateInput(nutrient);
                if (NutrientTargetType.From(nutrient.TargetType) == NutrientTargetType.EnergyPercent)
                {
                    totalPct += nutrient.MaxEnergyPct ?? 0;
                }
            }

            if (totalPct > 100)
            {
                throw new AppException(AppResponseCode.INVALID_ACTION, "Tổng thành phần dinh dưỡng không được vượt quá 100%");
            }

            var nutrientIds = request.Targets.Select(n => n.NutrientId).ToList();

            var nutrientsExist = await _nutrientRepository.IdsExistAsync(nutrientIds);

            if (!nutrientsExist)
                throw new AppException(AppResponseCode.NOT_FOUND, "Dinh dưỡng không tồn tại trong hệ thống");

            var goal = new HealthGoal
            {
                Name = request.Name,
                UpperName = upperName,
                Description = request.Description,
                Targets = request.Targets.Select(t => new HealthGoalTarget
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

            await _healthGoalRepository.AddAsync(goal);
            await _cache.RemoveByPrefixAsync("health-goal");
        }

        public async Task UpdateHealthGoalAsync(Guid id, UpdateHealthGoalRequest request)
        {
            var duplicateIds = request.Targets
                .GroupBy(t => t.NutrientId)
                .Where(g => g.Count() > 1)
                .Select(g => g.Key)
                .ToList();

            if (duplicateIds.Any())
            {
                throw new AppException(AppResponseCode.DUPLICATE, "Dinh dưỡng bị trùng lặp");
            }

            var totalPct = 0m;
            foreach (var nutrient in request.Targets)
            {
                await ValidateInput(nutrient);
                if (NutrientTargetType.From(nutrient.TargetType) == NutrientTargetType.EnergyPercent)
                {
                    totalPct += nutrient.MaxEnergyPct ?? 0;
                }
            }

            if (totalPct > 100)
            {
                throw new AppException(AppResponseCode.INVALID_ACTION, "Tổng thành phần dinh dưỡng không được vượt quá 100%");
            }

            var healthGoal = await _healthGoalRepository.GetByIdAsync(id, include: i => i.Include(h => h.Targets));

            if (healthGoal == null)
                throw new AppException(AppResponseCode.NOT_FOUND, "Mục tiêu sức khỏe không tồn tại");

            if (request.LastUpdatedUtc != healthGoal.LastUpdatedUtc)
            {
                throw new AppException(AppResponseCode.CONFLICT);
            }

            var nutrientIds = request.Targets.Select(n => n.NutrientId).ToList();

            var nutrientsExist = await _nutrientRepository.IdsExistAsync(nutrientIds);

            if (!nutrientsExist)
                throw new AppException(AppResponseCode.NOT_FOUND, "Dinh dưỡng không tồn tại");

            healthGoal.LastUpdatedUtc = DateTime.UtcNow;
            healthGoal.Description = request.Description;
            await _healthGoalTargetRepository.DeleteRangeAsync(healthGoal.Targets);
            healthGoal.Targets = request.Targets.Select(t => new HealthGoalTarget
            {
                NutrientId = t.NutrientId,
                TargetType = NutrientTargetType.From(t.TargetType),
                MinValue = t.MinValue,
                MaxValue = t.MaxValue,
                MinEnergyPct = t.MinEnergyPct,
                MaxEnergyPct = t.MaxEnergyPct,
                Weight = t.Weight
            }).ToList();

            await _healthGoalRepository.UpdateAsync(healthGoal);

            await _cache.RemoveByPrefixAsync("health-goal");
        }

        public async Task<IEnumerable<HealthGoalResponse>> GetHealthGoalsAsync()
        {
            const string cacheKey = "health-goal:system:list";

            var goals = await _healthGoalRepository.GetAllWithTargetsAsync();

            var result = _mapper.Map<IEnumerable<HealthGoalResponse>>(goals).OrderBy(u => u.Name);

            await _cache.SetAsync(cacheKey, result, TimeSpan.FromMinutes(30));

            return result;
        }

        public async Task<HealthGoalResponse> GetHealthGoalByIdAsync(Guid id)
        {
            var cacheKey = $"health-goal:system:detail:{id}";

            var goal = await _healthGoalRepository.GetByIdAsync(id,
                include: q => q.Include(g => g.Targets).ThenInclude(t => t.Nutrient));

            if (goal == null)
                throw new AppException(AppResponseCode.NOT_FOUND, "Mục tiêu dinh dưỡng không tồn tại");

            var result = _mapper.Map<HealthGoalResponse>(goal);

            await _cache.SetAsync(cacheKey, result, TimeSpan.FromMinutes(30));
            return result;

        }

        public async Task DeleteHealthGoalAsync(Guid id)
        {
            var goal = await _healthGoalRepository.GetByIdAsync(id,
                include: q => q.Include(g => g.Targets));

            if (goal == null)
                throw new AppException(AppResponseCode.NOT_FOUND);

            await _healthGoalTargetRepository.DeleteRangeAsync(goal.Targets);
            await _healthGoalRepository.DeleteAsync(goal);

            await _cache.RemoveByPrefixAsync("health-goal");
        }

        public async Task<IEnumerable<UserHealthGoalResponse>> GetListGoalAsync(Guid userId)
        {
            const string cacheKey = "health-goal:system:user-view";

            var result = new List<UserHealthGoalResponse>();

            var systemGoals = await _cache.GetAsync<List<UserHealthGoalResponse>>(cacheKey);

            if (systemGoals == null)
            {
                var healthGoals = await _healthGoalRepository.GetAllWithTargetsAsync();
                systemGoals = _mapper
                    .Map<List<UserHealthGoalResponse>>(healthGoals)
                    .OrderBy(x => x.Name)
                    .ToList();

                await _cache.SetAsync(cacheKey, systemGoals, TimeSpan.FromMinutes(30));
            }

            var customGoals = await _customHealthGoalRepository.GetByUserIdWithTargetsAsync(userId);
            var customs = _mapper.Map<IEnumerable<UserHealthGoalResponse>>(customGoals).OrderBy(x => x.Name);

            result.AddRange(customs);
            result.AddRange(systemGoals);

            return result.ToList();
        }

        private async Task ValidateInput(NutrientTargetRequest nutrientRequest)
        {
            var nutrient = await _nutrientRepository.GetByIdAsync(nutrientRequest.NutrientId);

            if (nutrientRequest.TargetType == NutrientTargetType.Absolute.Value)
            {
                if (nutrient!.IsMacroNutrient)
                    throw new AppException(AppResponseCode.INVALID_ACTION, "Không thể đặt mục tiêu chính xác cho dinh dưỡng đa lượng");

                if (!(nutrientRequest.MinValue.HasValue && nutrientRequest.MaxValue.HasValue))
                    throw new AppException(AppResponseCode.INVALID_ACTION, "Bạn phải nhập giá trị giới hạn cho dinh dưỡng");

                if (nutrientRequest.MaxValue < nutrientRequest.MinValue)
                    throw new AppException(AppResponseCode.INVALID_ACTION, "Giá trị tối đa phải lớn hơn giá trị tối thiểu");
            }
            else if (nutrientRequest.TargetType == NutrientTargetType.EnergyPercent.Value)
            {
                if (!nutrient!.IsMacroNutrient)
                    throw new AppException(AppResponseCode.INVALID_ACTION, "Không thể đặt mục tiêu phần trăm năng lượng cho dinh dưỡng vi lượng");

                if (!(nutrientRequest.MinEnergyPct.HasValue && nutrientRequest.MaxEnergyPct.HasValue))
                    throw new AppException(AppResponseCode.INVALID_ACTION, "Bạn phải nhập giá trị giới hạn cho dinh dưỡng");

                if (nutrientRequest.MaxEnergyPct < nutrientRequest.MinEnergyPct)
                    throw new AppException(AppResponseCode.INVALID_ACTION, "Giá trị tối đa phải lớn hơn giá trị tối thiểu");
            }
        }
    }
}
