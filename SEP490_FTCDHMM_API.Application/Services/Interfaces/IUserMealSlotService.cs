using SEP490_FTCDHMM_API.Application.Dtos.MealDtos;

namespace SEP490_FTCDHMM_API.Application.Services.Interfaces
{
    public interface IUserMealSlotService
    {
        Task<List<MealSlotResponse>> GetMyMealsAsync(Guid userId);
        Task CreateAsync(Guid userId, MealSlotRequest request);
        Task UpdateAsync(Guid userId, Guid slotId, MealSlotRequest request);
        Task DeleteAsync(Guid userId, Guid slotId);
    }
}
