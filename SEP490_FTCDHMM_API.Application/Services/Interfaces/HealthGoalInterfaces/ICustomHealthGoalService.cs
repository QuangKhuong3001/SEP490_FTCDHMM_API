using SEP490_FTCDHMM_API.Application.Dtos.CustomHealthGoalDtos;
using SEP490_FTCDHMM_API.Application.Dtos.HealthGoalDtos;

namespace SEP490_FTCDHMM_API.Application.Services.Interfaces.HealthGoalInterfaces
{
    public interface ICustomHealthGoalService
    {
        Task CreateAsync(Guid userId, CreateCustomHealthGoalRequest request);
        Task<HealthGoalResponse> GetByIdAsync(Guid userId, Guid id);
        Task UpdateAsync(Guid userId, Guid id, UpdateCustomHealthGoalRequest request);
        Task DeleteAsync(Guid userId, Guid id);
    }
}
