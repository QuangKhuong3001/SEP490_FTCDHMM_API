using SEP490_FTCDHMM_API.Domain.Entities;

namespace SEP490_FTCDHMM_API.Application.Interfaces
{
    public interface IRoleRepository : IRepository<AppRole>
    {
        Task<AppRole?> GetRoleWithPermissionsAsync(Guid roleId);
        Task<AppRole?> FindByNameAsync(string roleName);

    }
}
