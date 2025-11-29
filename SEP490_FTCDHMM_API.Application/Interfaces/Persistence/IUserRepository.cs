using SEP490_FTCDHMM_API.Domain.Entities;

namespace SEP490_FTCDHMM_API.Application.Interfaces.Persistence
{
    public interface IUserRepository : IRepository<AppUser>
    {
        Task<AppUser?> GetByUserNameAsync(string userName);
        Task<List<AppUser>> GetTaggableUsersAsync(Guid userId, string? keyword);
    }
}
