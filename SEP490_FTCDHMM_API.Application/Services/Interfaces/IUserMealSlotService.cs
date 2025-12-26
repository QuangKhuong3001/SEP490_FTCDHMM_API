using SEP490_FTCDHMM_API.Application.Dtos.MealDtos;

namespace SEP490_FTCDHMM_API.Application.Services.Interfaces
{
    public interface IUserMealSlotService
    {
        Task<List<MealSlotResponse>> GetMyMealsAsync(Guid userId);
        Task CreateMealSlotAsync(Guid userId, MealSlotRequest request);
        Task UpdateMealSlotAsync(Guid userId, Guid slotId, MealSlotRequest request);
        Task DeleteMealSlotAsync(Guid userId, Guid slotId);
    }
}
