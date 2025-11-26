using SEP490_FTCDHMM_API.Domain.Entities;

namespace SEP490_FTCDHMM_API.Application.Interfaces.Persistence
{
    public interface IUserBehaviorRepository : IRepository<UserLabelStat>
    {
        Task<UserLabelStat?> GetAsync(Guid userId, Guid labelId);
        Task<IEnumerable<UserLabelStat>> GetAllForUserAsync(Guid userId);
    }
}
