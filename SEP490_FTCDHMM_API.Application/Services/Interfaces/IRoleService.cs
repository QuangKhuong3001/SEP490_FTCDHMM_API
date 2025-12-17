using SEP490_FTCDHMM_API.Application.Dtos.Common;
using SEP490_FTCDHMM_API.Application.Dtos.RoleDtos;

namespace SEP490_FTCDHMM_API.Application.Services.Interfaces
{
    public interface IRoleService
    {
        Task CreateRoleAsync(CreateRoleRequest dto);
        Task DeleteRoleAsync(Guid roleId);
        Task<PagedResult<RoleResponse>> GetRolesAsync(PaginationParams pagination);
        Task<List<RoleNameResponse>> GetActivateRolesAsync();
        Task ActiveRoleAsync(Guid roleId);
        Task DeactiveRoleAsync(Guid roleId);
        Task UpdateRolePermissionsAsync(Guid roleId, RolePermissionSettingRequest dto);
        Task<RoleDetailsResponse> GetRolePermissionsAsync(Guid roleId);

    }
}
