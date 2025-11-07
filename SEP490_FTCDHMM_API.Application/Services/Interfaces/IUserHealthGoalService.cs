using SEP490_FTCDHMM_API.Application.Dtos.HealthGoalDtos;

namespace SEP490_FTCDHMM_API.Application.Services.Interfaces
{

    public interface IUserHealthGoalService
    {
        Task SetGoalAsync(Guid userId, Guid healthGoalId);
        Task<IEnumerable<HealthGoalResponse>> GetCurrentGoalAsync(Guid userId);
        Task RemoveFromCurrent(Guid userId, Guid healthGoalId);
    }
}
