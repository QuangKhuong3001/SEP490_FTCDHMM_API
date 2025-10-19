using SEP490_FTCDHMM_API.Application.Dtos.Common;
using SEP490_FTCDHMM_API.Application.Dtos.RoleDtos;

namespace SEP490_FTCDHMM_API.Application.Services.Interfaces
{
    public interface IRoleService
    {
        Task CreateRole(CreateRoleRequest dto);
        Task DeleteRole(Guid roleId);
        Task<PagedResult<RoleResponse>> GetAllRoles(PaginationParams pagination);
        Task<RoleNameResponse> GetActivateRoles();
        Task ActiveRole(Guid roleId);
        Task DeactiveRole(Guid roleId);
        Task UpdateRolePermissions(Guid roleId, RolePermissionSettingRequest dto);
        Task<List<PermissionDomainRequest>> GetRolePermissions(Guid roleId);

    }
}
