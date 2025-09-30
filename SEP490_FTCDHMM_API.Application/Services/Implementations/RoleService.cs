using AutoMapper;
using SEP490_FTCDHMM_API.Application.Dtos.Common;
using SEP490_FTCDHMM_API.Application.Dtos.RoleDtos;
using SEP490_FTCDHMM_API.Application.Interfaces;
using SEP490_FTCDHMM_API.Application.Services.Interfaces;
using SEP490_FTCDHMM_API.Domain.Entities;
using SEP490_FTCDHMM_API.Shared.Exceptions;

namespace SEP490_FTCDHMM_API.Application.Services.Implementations
{
    public class RoleService : IRoleService
    {
        private readonly IRoleRepository _roleRepository;
        private readonly IUserRepository _userRepository;
        private readonly IPermissionActionRepository _permissionActionRepository;
        private readonly IPermissionDomainRepository _permissionDomainRepository;
        private readonly IRolePermissionRepository _rolePermissionRepository;
        private readonly IMapper _mapper;

        public RoleService(
            IRoleRepository roleRepository,
            IUserRepository userRepository,
            IPermissionActionRepository permissionActionRepository,
            IPermissionDomainRepository permissionDomainRepository,
            IRolePermissionRepository rolePermissionRepository,
            IMapper mapper)
        {
            _roleRepository = roleRepository;
            _userRepository = userRepository;
            _permissionActionRepository = permissionActionRepository;
            _permissionDomainRepository = permissionDomainRepository;
            _rolePermissionRepository = rolePermissionRepository;
            _mapper = mapper;
        }

        public async Task CreateRole(CreateRoleDto dto)
        {
            var existing = await _roleRepository.ExistsAsync(r => r.Name == dto.Name);

            if (existing)
                throw new AppException(AppResponseCode.ROLE_ALREADY_EXISTS);

            var role = new AppRole
            {
                Id = Guid.NewGuid(),
                NormalizedName = dto.Name.ToUpperInvariant(),
                IsActive = true,
                Name = dto.Name
            };

            await _roleRepository.AddAsync(role);

            var allPermissions = await _permissionActionRepository.GetAllAsync();

            var rolePermissions = allPermissions.Select(p => new AppRolePermission
            {
                RoleId = role.Id,
                PermissionActionId = p.Id,
                IsActive = false
            }).ToList();

            await _rolePermissionRepository.AddRangeAsync(rolePermissions);
        }

        public async Task DeleteRole(Guid roleId)
        {
            var role = await _roleRepository.GetByIdAsync(roleId);
            if (role == null)
                throw new AppException(AppResponseCode.NOT_FOUND);

            var existingUserInRole = await _userRepository.ExistsAsync(x => x.RoleId == roleId);
            if (existingUserInRole)
                throw new AppException(AppResponseCode.INVALID_ACTION);

            await _roleRepository.DeleteAsync(role);
        }

        public async Task<PagedResult<RoleDto>> GetAllRoles(PaginationParams pagination)
        {
            var (roles, totalCount) = await _roleRepository.GetPagedAsync(
                pagination.Page, pagination.PageSize);

            var result = _mapper.Map<List<RoleDto>>(roles);
            return new PagedResult<RoleDto>
            {
                Items = result,
                TotalCount = totalCount,
                Page = pagination.Page,
                PageSize = pagination.PageSize
            };
        }

        public async Task ActiveRole(Guid roleId)
        {
            var role = await _roleRepository.GetByIdAsync(roleId);
            if (role == null)
                throw new AppException(AppResponseCode.NOT_FOUND);

            if (role.IsActive == true)
                throw new AppException(AppResponseCode.INVALID_ACTION);

            role.IsActive = true;
            await _roleRepository.UpdateAsync(role);

        }
        public async Task DeactiveRole(Guid roleId)
        {
            var role = await _roleRepository.GetByIdAsync(roleId);
            if (role == null)
                throw new AppException(AppResponseCode.NOT_FOUND);

            if (role.IsActive == false)
                throw new AppException(AppResponseCode.INVALID_ACTION);

            role.IsActive = false;
            await _roleRepository.UpdateAsync(role);
        }

        public async Task UpdateRolePermissions(RolePermissionSettingDto dto)
        {
            var rolePermissions = await _rolePermissionRepository
                .GetAllAsync(rp => rp.RoleId == dto.RoleId);

            if (rolePermissions == null || !rolePermissions.Any())
                throw new AppException(AppResponseCode.NOT_FOUND);

            foreach (var permission in dto.Permissions)
            {
                var rp = rolePermissions.FirstOrDefault(x => x.PermissionActionId == permission.PermissionActionId);
                if (rp != null)
                {
                    rp.IsActive = permission.IsActive;
                    await _rolePermissionRepository.UpdateAsync(rp);
                }
            }
        }

        public async Task<List<PermissionDomainDto>> GetRolePermissions(Guid roleId)
        {
            var domains = await _permissionDomainRepository.GetAllAsync(d => d.Actions);

            var rolePermissions = await _rolePermissionRepository.GetAllAsync(rp => rp.RoleId == roleId);

            var result = domains.Select(d => new PermissionDomainDto
            {
                DomainName = d.Name,
                Actions = d.Actions.Select(a => new PermissionActionDto
                {
                    ActionId = a.Id,
                    ActionName = a.Name,
                    IsActive = rolePermissions.Any(rp =>
                        rp.PermissionActionId == a.Id && rp.IsActive)
                }).ToList()
            }).ToList();

            return result;
        }

    }
}
