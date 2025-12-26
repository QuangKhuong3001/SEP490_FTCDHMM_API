using AutoMapper;
using SEP490_FTCDHMM_API.Application.Dtos.MealDtos;
using SEP490_FTCDHMM_API.Application.Interfaces.Persistence;
using SEP490_FTCDHMM_API.Application.Services.Interfaces;
using SEP490_FTCDHMM_API.Domain.Entities;
using SEP490_FTCDHMM_API.Shared.Exceptions;

namespace SEP490_FTCDHMM_API.Application.Services.Implementations
{
    public class UserMealSlotService : IUserMealSlotService
    {
        private readonly IMapper _mapper;
        private readonly IMealSlotRepository _mealSlotRepository;

        public UserMealSlotService(IMapper mapper, IMealSlotRepository mealSlotRepository)
        {
            _mapper = mapper;
            _mealSlotRepository = mealSlotRepository;
        }

        public async Task CreateMealSlotAsync(Guid userId, MealSlotRequest request)
        {
            var existing = await _mealSlotRepository.GetByUserAsync(userId);
            if (existing.Any(x => x.OrderIndex == request.OrderIndex))
                throw new AppException(AppResponseCode.INVALID_ACTION);

            var sum = existing.Sum(x => x.EnergyPercent) + request.EnergyPercent;
            if (sum > 100)
                throw new AppException(AppResponseCode.INVALID_ACTION, "Tổng năng lượng không vượt quá 100%");

            var slot = new UserMealSlot
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Name = request.Name.Trim(),
                EnergyPercent = request.EnergyPercent,
                OrderIndex = request.OrderIndex
            };

            await _mealSlotRepository.AddAsync(slot);
        }

        public async Task DeleteMealSlotAsync(Guid userId, Guid slotId)
        {
            var slot = await _mealSlotRepository.GetByIdAsync(slotId);
            if (slot == null || slot.UserId != userId)
                throw new AppException(AppResponseCode.NOT_FOUND);

            await _mealSlotRepository.DeleteAsync(slot);
        }

        public async Task<List<MealSlotResponse>> GetMyMealsAsync(Guid userId)
        {
            var slots = await _mealSlotRepository.GetByUserAsync(userId);
            return _mapper.Map<List<MealSlotResponse>>(slots.OrderBy(x => x.OrderIndex));
        }

        public async Task UpdateMealSlotAsync(Guid userId, Guid slotId, MealSlotRequest request)
        {
            var slot = await _mealSlotRepository.GetByIdAsync(slotId);
            if (slot == null || slot.UserId != userId)
                throw new AppException(AppResponseCode.NOT_FOUND);

            var all = await _mealSlotRepository.GetByUserAsync(userId);
            if (all.Any(x => x.OrderIndex == request.OrderIndex))
                throw new AppException(AppResponseCode.INVALID_ACTION);

            var sum = all.Where(x => x.Id != slotId).Sum(x => x.EnergyPercent) + request.EnergyPercent;
            if (sum > 100)
                throw new AppException(AppResponseCode.INVALID_ACTION, "Tổng năng lượng không vượt quá 100%");

            slot.Name = request.Name.Trim();
            slot.EnergyPercent = request.EnergyPercent;
            slot.OrderIndex = request.OrderIndex;

            await _mealSlotRepository.UpdateAsync(slot);
        }
    }
}
