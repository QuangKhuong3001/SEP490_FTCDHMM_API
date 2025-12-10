using SEP490_FTCDHMM_API.Application.Dtos.HealthGoalDtos;
using SEP490_FTCDHMM_API.Application.Dtos.UserHealthGoalDtos;

namespace SEP490_FTCDHMM_API.Application.Services.Interfaces.HealthGoalInterfaces
{
    public interface IHealthGoalService
    {
        Task CreateHealthGoalAsync(CreateHealthGoalRequest request);
        Task UpdateHealthGoalAsync(Guid id, UpdateHealthGoalRequest request);
        Task<IEnumerable<HealthGoalResponse>> GetHealthGoalsAsync();
        Task<HealthGoalResponse> GetHealthGoalByIdAsync(Guid id);
        Task DeleteHealthGoalAsync(Guid id);
        Task<IEnumerable<UserHealthGoalResponse>> GetListGoalAsync(Guid userId);

    }
}
