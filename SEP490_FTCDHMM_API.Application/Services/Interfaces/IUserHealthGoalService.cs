using SEP490_FTCDHMM_API.Application.Dtos.HealthGoalDtos;
using SEP490_FTCDHMM_API.Application.Dtos.UserHealthGoalDtos;

namespace SEP490_FTCDHMM_API.Application.Services.Interfaces
{

    public interface IUserHealthGoalService
    {
        Task SetGoalAsync(Guid userId, Guid targetId, UserHealthGoalRequest request);
        Task<UserHealthGoalResponse> GetCurrentGoalAsync(Guid userId);
        Task RemoveFromCurrent(Guid userId, Guid healthGoalId);
    }
}
