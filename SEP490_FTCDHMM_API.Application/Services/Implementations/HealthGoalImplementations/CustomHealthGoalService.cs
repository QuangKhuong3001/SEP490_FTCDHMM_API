using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SEP490_FTCDHMM_API.Application.Dtos.CustomHealthGoalDtos;
using SEP490_FTCDHMM_API.Application.Dtos.HealthGoalDtos;
using SEP490_FTCDHMM_API.Application.Dtos.NutrientDtos.NutrientTarget;
using SEP490_FTCDHMM_API.Application.Interfaces.Persistence;
using SEP490_FTCDHMM_API.Application.Services.Interfaces.HealthGoalInterfaces;
using SEP490_FTCDHMM_API.Domain.Entities;
using SEP490_FTCDHMM_API.Domain.ValueObjects;
using SEP490_FTCDHMM_API.Shared.Exceptions;

namespace SEP490_FTCDHMM_API.Application.Services.Implementations.HealthGoalImplementations
{
    public class CustomHealthGoalService : ICustomHealthGoalService
    {
        private readonly ICustomHealthGoalRepository _customHealthGoalRepository;
        private readonly IMapper _mapper;
        private readonly INutrientRepository _nutrientRepository;
        private readonly IHealthGoalTargetRepository _healthGoalTargetRepository;

        public CustomHealthGoalService(
            ICustomHealthGoalRepository customHealthGoalRepository,
            IMapper mapper,
            IHealthGoalTargetRepository healthGoalTargetRepository,
            INutrientRepository nutrientRepository)
        {
            _customHealthGoalRepository = customHealthGoalRepository;
            _mapper = mapper;
            _healthGoalTargetRepository = healthGoalTargetRepository;
            _nutrientRepository = nutrientRepository;
        }

        public async Task CreateCustomHealthGoalAsync(Guid userId, CreateCustomHealthGoalRequest request)
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
                await this.ValidateInput(nutrient);
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

            var exist = await _nutrientRepository.IdsExistAsync(nutrientIds);

            if (!exist)
                throw new AppException(AppResponseCode.NOT_FOUND, "Dinh dưỡng không tồn tại");

            var goal = new CustomHealthGoal
            {
                UserId = userId,
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

            await _customHealthGoalRepository.AddAsync(goal);
        }

        public async Task UpdateCustomHealthGoalAsync(Guid userId, Guid customHealthGoalId, UpdateCustomHealthGoalRequest request)
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
                await this.ValidateInput(nutrient);
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

            var exist = await _nutrientRepository.IdsExistAsync(nutrientIds);

            if (!exist)
                throw new AppException(AppResponseCode.NOT_FOUND, "Dinh dưỡng không tồn tại");

            var healthGoal = await _customHealthGoalRepository.GetByIdAsync(customHealthGoalId,
                include: i => i.Include(h => h.Targets));

            if (healthGoal == null)
                throw new AppException(AppResponseCode.NOT_FOUND);

            if (healthGoal.UserId != userId)
                throw new AppException(AppResponseCode.FORBIDDEN);

            await _healthGoalTargetRepository.DeleteRangeAsync(healthGoal.Targets);

            healthGoal.Name = request.Name;
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

            await _customHealthGoalRepository.UpdateAsync(healthGoal);
        }

        public async Task<HealthGoalResponse> GetCustomHealthGoalByIdAsync(Guid userId, Guid customHealthGoalId)
        {
            var goal = await _customHealthGoalRepository.GetByIdAsync(customHealthGoalId,
                include: q => q.Include(g => g.Targets).ThenInclude(t => t.Nutrient));

            if (goal == null)
                throw new AppException(AppResponseCode.NOT_FOUND);

            if (goal.UserId != userId)
                throw new AppException(AppResponseCode.FORBIDDEN);

            var result = _mapper.Map<HealthGoalResponse>(goal);
            return result;
        }

        public async Task DeleteCustomHealthGoalAsync(Guid userId, Guid customHealthGoalId)
        {
            var goal = await _customHealthGoalRepository.GetByIdAsync(customHealthGoalId,
                include: q => q.Include(g => g.Targets));

            if (goal == null)
                throw new AppException(AppResponseCode.NOT_FOUND);

            if (goal.UserId != userId)
                throw new AppException(AppResponseCode.FORBIDDEN);


            await _healthGoalTargetRepository.DeleteRangeAsync(goal.Targets);
            await _customHealthGoalRepository.DeleteAsync(goal);
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
