using SEP490_FTCDHMM_API.Application.Dtos.UserHealthGoalDtos;

namespace SEP490_FTCDHMM_API.Application.Services.Interfaces.HealthGoalInterfaces
{

    public interface IUserHealthGoalService
    {
        Task SetGoalAsync(Guid userId, Guid targetId, UserHealthGoalRequest request);
        Task<UserHealthGoalResponse> GetCurrentGoalAsync(Guid userId);
        Task<IEnumerable<UserHealthGoalResponse>> GetHistoryGoalAsync(Guid userId);
        Task RemoveFromCurrent(Guid userId);
    }
}
