using SEP490_FTCDHMM_API.Application.Dtos.CustomHealthGoalDtos;
using SEP490_FTCDHMM_API.Application.Dtos.HealthGoalDtos;

namespace SEP490_FTCDHMM_API.Application.Services.Interfaces.HealthGoalInterfaces
{
    public interface ICustomHealthGoalService
    {
        Task CreateCustomHealthGoalAsync(Guid userId, CreateCustomHealthGoalRequest request);
        Task<HealthGoalResponse> GetCustomHealthGoalByIdAsync(Guid userId, Guid id);
        Task UpdateCustomHealthGoalAsync(Guid userId, Guid id, UpdateCustomHealthGoalRequest request);
        Task DeleteCustomHealthGoalAsync(Guid userId, Guid id);
    }
}
