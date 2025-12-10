using SEP490_FTCDHMM_API.Application.Dtos.UserHealthMetricDtos;

namespace SEP490_FTCDHMM_API.Application.Services.Interfaces
{
    public interface IUserHealthMetricService
    {
        Task CreateHealthMetricAsync(Guid userId, CreateUserHealthMetricRequest request);
        Task UpdateHealthMetricAsync(Guid userId, Guid metricId, UpdateUserHealthMetricRequest request);
        Task DeleteHealthMetricAsync(Guid userId, Guid metricId);
        Task<IEnumerable<UserHealthMetricResponse>> GetHealthMetricHistoryByUserIdAsync(Guid userId);
    }
}
