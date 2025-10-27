using SEP490_FTCDHMM_API.Application.Dtos.HealthGoalDtos;

namespace SEP490_FTCDHMM_API.Application.Services.Interfaces
{
    public interface IHealthGoalService
    {
        Task CreateAsync(CreateHealthGoalRequest request);
        Task UpdateAsync(Guid id, UpdateHealthGoalRequest request);
        Task<IReadOnlyList<HealthGoalResponse>> GetAllAsync();
        Task<HealthGoalResponse> GetByIdAsync(Guid id);
        Task DeleteAsync(Guid id);
    }
}
