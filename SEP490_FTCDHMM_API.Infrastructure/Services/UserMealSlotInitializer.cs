using Microsoft.Extensions.Options;
using SEP490_FTCDHMM_API.Application.Interfaces.Persistence;
using SEP490_FTCDHMM_API.Application.Interfaces.SystemServices;
using SEP490_FTCDHMM_API.Domain.Entities;
using SEP490_FTCDHMM_API.Infrastructure.ModelSettings;

namespace SEP490_FTCDHMM_API.Application.Services.Implementations
{
    public class UserMealSlotInitializer : IUserMealSlotInitializer
    {
        private readonly IMealSlotRepository _mealSlotRepository;
        private readonly MealDistributionSettings _mealDistributionSettings;

        public UserMealSlotInitializer(IMealSlotRepository mealSlotRepository, IOptions<MealDistributionSettings> mealDistributionSettings)
        {
            _mealDistributionSettings = mealDistributionSettings.Value;
            _mealSlotRepository = mealSlotRepository;
        }

        public async Task InitializeDefaultAsync(Guid userId)
        {
            var exists = await _mealSlotRepository.GetByUserAsync(userId);
            if (exists.Any())
                return;

            var slots = new[]
            {
                new UserMealSlot
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    Name = "Breakfast",
                    EnergyPercent = _mealDistributionSettings.Breakfast,
                    OrderIndex = 1
                },
                new UserMealSlot
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    Name = "Lunch",
                    EnergyPercent = _mealDistributionSettings.Lunch,
                    OrderIndex = 2
                },
                new UserMealSlot
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    Name = "Dinner",
                    EnergyPercent = _mealDistributionSettings.Dinner,
                    OrderIndex = 3
                }
            };

            foreach (var slot in slots)
                await _mealSlotRepository.AddAsync(slot);
        }
    }
}
