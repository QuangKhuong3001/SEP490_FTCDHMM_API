using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SEP490_FTCDHMM_API.Application.Dtos.HealthGoalDtos;
using SEP490_FTCDHMM_API.Application.Dtos.NutrientDtos.NutrientTarget;
using SEP490_FTCDHMM_API.Application.Dtos.UserHealthGoalDtos;
using SEP490_FTCDHMM_API.Application.Interfaces.Persistence;
using SEP490_FTCDHMM_API.Application.Services.Interfaces.HealthGoalInterfaces;
using SEP490_FTCDHMM_API.Domain.Entities;
using SEP490_FTCDHMM_API.Domain.ValueObjects;
using SEP490_FTCDHMM_API.Shared.Exceptions;

namespace SEP490_FTCDHMM_API.Application.Services.Implementations.HealthGoalImplementations
{
    public class HealthGoalService : IHealthGoalService
    {
        private readonly IHealthGoalRepository _healthGoalRepository;
        private readonly INutrientRepository _nutrientRepository;
        private readonly IMapper _mapper;
        private readonly ICustomHealthGoalRepository _customHealthGoalRepository;

        public HealthGoalService(IHealthGoalRepository healthGoalRepository,
            INutrientRepository nutrientRepository,
            IMapper mapper, ICustomHealthGoalRepository customHealthGoalRepository)
        {
            _healthGoalRepository = healthGoalRepository;
            _nutrientRepository = nutrientRepository;
            _mapper = mapper;
            _customHealthGoalRepository = customHealthGoalRepository;
        }

        public async Task CreateAsync(CreateHealthGoalRequest request)
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
                IsValidInput(nutrient);
                totalPct += nutrient.MaxEnergyPct ?? 0;
            }

            if (totalPct > 100)
            {
                throw new AppException(AppResponseCode.INVALID_ACTION, "Tổng thành phần dinh dưỡng không được vượt quá 100%");
            }

            var nutrientIds = request.Targets.Select(n => n.NutrientId).ToList();

            var exist = await _nutrientRepository.IdsExistAsync(nutrientIds);

            if (!exist)
                throw new AppException(AppResponseCode.NOT_FOUND, "Dinh dưỡng không tồn tại trong hệ thống");

            var goal = new HealthGoal
            {
                Name = request.Name,
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
        }

        public async Task UpdateAsync(Guid id, UpdateHealthGoalRequest request)
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
                IsValidInput(nutrient);
                totalPct += nutrient.MaxEnergyPct ?? 0;
            }

            if (totalPct > 100)
            {
                throw new AppException(AppResponseCode.INVALID_ACTION, "Tổng thành phần dinh dưỡng không được vượt quá 100%");
            }

            var healthGoal = await _healthGoalRepository.GetByIdAsync(id, include: i => i.Include(h => h.Targets));

            if (healthGoal == null)
                throw new AppException(AppResponseCode.NOT_FOUND, "Mục tiêu sức khỏe không tồn tại");

            var nutrientIds = request.Targets.Select(n => n.NutrientId).ToList();

            var exist = await _nutrientRepository.IdsExistAsync(nutrientIds);

            if (!exist)
                throw new AppException(AppResponseCode.NOT_FOUND, "Dinh dưỡng không tồn tại");

            healthGoal.Targets.Clear();

            healthGoal.Description = request.Description;
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
        }

        public async Task<IEnumerable<HealthGoalResponse>> GetAllAsync()
        {
            var goals = await _healthGoalRepository.GetAllAsync(
                include: q => q.Include(g => g.Targets).ThenInclude(t => t.Nutrient)

            );

            var result = _mapper.Map<IEnumerable<HealthGoalResponse>>(goals).OrderBy(u => u.Name);

            return result;
        }

        public async Task<HealthGoalResponse> GetByIdAsync(Guid id)
        {
            var goal = await _healthGoalRepository.GetByIdAsync(id,
                include: q => q.Include(g => g.Targets).ThenInclude(t => t.Nutrient));

            if (goal == null)
                throw new AppException(AppResponseCode.NOT_FOUND, "Mục tiêu dinh dưỡng không tồn tại");

            var result = _mapper.Map<HealthGoalResponse>(goal);
            return result;

        }

        public async Task DeleteAsync(Guid id)
        {
            var goal = await _healthGoalRepository.GetByIdAsync(id,
                include: q => q.Include(g => g.Targets));

            if (goal == null)
                throw new AppException(AppResponseCode.NOT_FOUND);

            goal.Targets.Clear();

            await _healthGoalRepository.DeleteAsync(goal);
        }

        public async Task<IEnumerable<UserHealthGoalResponse>> GetListGoalAsync(Guid userId)
        {
            var result = new List<UserHealthGoalResponse>();

            var healthGoals = await _healthGoalRepository.GetAllWithTargetsAsync();
            var defaults = _mapper.Map<IEnumerable<UserHealthGoalResponse>>(healthGoals).OrderBy(x => x.Name);

            var customGoals = await _customHealthGoalRepository.GetByUserIdWithTargetsAsync(userId);
            var customs = _mapper.Map<IEnumerable<UserHealthGoalResponse>>(customGoals).OrderBy(x => x.Name);

            result.AddRange(customs);
            result.AddRange(defaults);

            return result.ToList();
        }

        private bool IsValidInput(NutrientTargetRequest nutrient)
        {
            if (nutrient.TargetType == NutrientTargetType.Absolute.Value)
            {
                if (!(nutrient.MinValue.HasValue && nutrient.MaxValue.HasValue))
                    throw new AppException(AppResponseCode.INVALID_ACTION, "Bạn phải nhập giá trị giới hạn cho dinh dưỡng");

                if (nutrient.MaxValue <= nutrient.MinValue)
                    throw new AppException(AppResponseCode.INVALID_ACTION, "Giá trị tối đa phải lớn hơn giá trị tối thiểu");
            }
            else if (nutrient.TargetType == NutrientTargetType.EnergyPercent.Value)
            {
                if (!(nutrient.MinEnergyPct.HasValue && nutrient.MaxEnergyPct.HasValue))
                    throw new AppException(AppResponseCode.INVALID_ACTION, "Bạn phải nhập giá trị giới hạn cho dinh dưỡng");

                if (nutrient.MaxEnergyPct <= nutrient.MinEnergyPct)
                    throw new AppException(AppResponseCode.INVALID_ACTION, "Giá trị tối đa phải lớn hơn giá trị tối thiểu");
            }

            return false;
        }
    }
}
