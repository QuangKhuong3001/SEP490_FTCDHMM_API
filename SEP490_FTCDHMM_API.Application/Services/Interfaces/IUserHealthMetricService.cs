using SEP490_FTCDHMM_API.Application.Dtos.UserHealthMetricDtos;

namespace SEP490_FTCDHMM_API.Application.Services.Interfaces
{
    public interface IUserHealthMetricService
    {
        Task CreateAsync(Guid userId, CreateUserHealthMetricRequest request);
        Task UpdateAsync(Guid userId, Guid metricId, UpdateUserHealthMetricRequest request);
        Task DeleteAsync(Guid userId, Guid metricId);
        Task<IEnumerable<UserHealthMetricResponse>> GetHistoryByUserIdAsync(Guid userId);
        Task RecalculateUserMetricsAsync(Guid userId);
    }
}
