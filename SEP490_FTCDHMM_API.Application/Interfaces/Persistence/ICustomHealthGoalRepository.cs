using SEP490_FTCDHMM_API.Domain.Entities;

namespace SEP490_FTCDHMM_API.Application.Interfaces.Persistence
{
    public interface ICustomHealthGoalRepository : IRepository<CustomHealthGoal>
    {
        Task<List<CustomHealthGoal>> GetByUserIdWithTargetsAsync(Guid userId);
    }
}
