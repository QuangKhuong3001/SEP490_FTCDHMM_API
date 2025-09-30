using SEP490_FTCDHMM_API.Application.Dtos.Common;
using SEP490_FTCDHMM_API.Application.Dtos.RoleDtos;

namespace SEP490_FTCDHMM_API.Application.Services.Interfaces
{
    public interface IRoleService
    {
        Task CreateRole(CreateRoleDto dto);
        Task DeleteRole(Guid roleId);
        Task<PagedResult<RoleDto>> GetAllRoles(PaginationParams pagination);
        Task ActiveRole(Guid roleId);
        Task DeactiveRole(Guid roleId);
        Task UpdateRolePermissions(RolePermissionSettingDto dto);
        Task<List<PermissionDomainDto>> GetRolePermissions(Guid roleId);

    }
}
