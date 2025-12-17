using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SEP490_FTCDHMM_API.Application.Dtos.Common;
using SEP490_FTCDHMM_API.Application.Dtos.RoleDtos;
using SEP490_FTCDHMM_API.Application.Interfaces.Persistence;
using SEP490_FTCDHMM_API.Application.Services.Implementations.SEP490_FTCDHMM_API.Application.Interfaces;
using SEP490_FTCDHMM_API.Application.Services.Interfaces;
using SEP490_FTCDHMM_API.Domain.Constants;
using SEP490_FTCDHMM_API.Domain.Entities;
using SEP490_FTCDHMM_API.Shared.Exceptions;
using SEP490_FTCDHMM_API.Shared.Utils;

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
        private readonly ICacheService _cacheService;

        public RoleService(
            IRoleRepository roleRepository,
            IUserRepository userRepository,
            IPermissionActionRepository permissionActionRepository,
            IPermissionDomainRepository permissionDomainRepository,
            IRolePermissionRepository rolePermissionRepository,
            ICacheService cache,
            IMapper mapper)
        {
            _roleRepository = roleRepository;
            _userRepository = userRepository;
            _permissionActionRepository = permissionActionRepository;
            _permissionDomainRepository = permissionDomainRepository;
            _rolePermissionRepository = rolePermissionRepository;
            _cacheService = cache;
            _mapper = mapper;
        }

        public async Task CreateRoleAsync(CreateRoleRequest dto)
        {
            var existing = await _roleRepository.ExistsAsync(r => r.NormalizedName == dto.Name.ToUpperInvariant().CleanDuplicateSpace());

            if (existing)
                throw new AppException(AppResponseCode.EXISTS);

            var role = new AppRole
            {
                NormalizedName = dto.Name.ToUpperInvariant().CleanDuplicateSpace(),
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
            await _cacheService.RemoveByPrefixAsync("role");
        }

        public async Task DeleteRoleAsync(Guid roleId)
        {
            var role = await _roleRepository.GetByIdAsync(roleId);
            if (role == null)
                throw new AppException(AppResponseCode.NOT_FOUND);

            if (role.Name == RoleConstants.Admin)
                throw new AppException(AppResponseCode.INVALID_ACTION, "Không được quyền chỉnh sửa tài khoàn admin");

            var existingUserInRole = await _userRepository.ExistsAsync(x => x.RoleId == roleId);
            if (existingUserInRole)
                throw new AppException(AppResponseCode.INVALID_ACTION, "Hiện đang có người dùng sử dụng vai trò này");

            await _roleRepository.DeleteAsync(role);
            await _cacheService.RemoveByPrefixAsync("role");
        }

        public async Task<PagedResult<RoleResponse>> GetRolesAsync(PaginationParams pagination)
        {
            var (roles, totalCount) = await _roleRepository.GetPagedAsync(
                pagination.PageNumber, pagination.PageSize);

            var result = _mapper.Map<List<RoleResponse>>(roles).OrderBy(r => r.Name);

            return new PagedResult<RoleResponse>
            {
                Items = result,
                TotalCount = totalCount,
                PageNumber = pagination.PageNumber,
                PageSize = pagination.PageSize
            };
        }

        public async Task<List<RoleNameResponse>> GetActivateRolesAsync()
        {
            const string cacheKey = "role:active:list";

            var cached = await _cacheService.GetAsync<List<RoleNameResponse>>(cacheKey);
            if (cached != null)
                return cached;

            var roles = await _roleRepository.GetAllAsync(r => r.IsActive);

            var result = _mapper.Map<List<RoleNameResponse>>(roles);

            await _cacheService.SetAsync(cacheKey, result, TimeSpan.FromMinutes(30));

            return result;
        }


        public async Task ActiveRoleAsync(Guid roleId)
        {
            var role = await _roleRepository.GetByIdAsync(roleId);
            if (role == null)
                throw new AppException(AppResponseCode.NOT_FOUND);

            if (role.IsActive == true)
                throw new AppException(AppResponseCode.INVALID_ACTION);

            role.IsActive = true;
            await _roleRepository.UpdateAsync(role);
            await _cacheService.RemoveByPrefixAsync("role");
        }
        public async Task DeactiveRoleAsync(Guid roleId)
        {
            var role = await _roleRepository.GetByIdAsync(roleId);
            if (role == null)
                throw new AppException(AppResponseCode.NOT_FOUND);

            if (role.IsActive == false)
                throw new AppException(AppResponseCode.INVALID_ACTION);

            if (role.Name == RoleConstants.Admin)
                throw new AppException(AppResponseCode.INVALID_ACTION, "Không được quyền chỉnh sửa tài khoàn admin");

            var existingUserInRole = await _userRepository.ExistsAsync(x => x.RoleId == roleId);
            if (existingUserInRole)
            {
                throw new AppException(AppResponseCode.INVALID_ACTION, "Vai trò này đang được gắn cho một hoặc nhiều người dùng");
            }

            role.IsActive = false;
            await _roleRepository.UpdateAsync(role);
            await _cacheService.RemoveByPrefixAsync("role");
        }

        public async Task UpdateRolePermissionsAsync(Guid roleId, RolePermissionSettingRequest dto)
        {
            var role = await _roleRepository.GetByIdAsync(roleId,
                include: i => i.Include(r => r.RolePermissions));

            if (role == null)
                throw new AppException(AppResponseCode.NOT_FOUND);

            if (role.LastUpdatedUtc != dto.LastUpdatedUtc)
                throw new AppException(AppResponseCode.CONFLICT);

            if (role.Name == RoleConstants.Admin)
                throw new AppException(AppResponseCode.INVALID_ACTION, "Không được quyền chỉnh sửa tài khoàn admin");

            var rolePermissions = role.RolePermissions;
            role.LastUpdatedUtc = DateTime.UtcNow;

            foreach (var permission in dto.Permissions)
            {
                var rp = rolePermissions.FirstOrDefault(
                    x => x.PermissionActionId == permission.PermissionActionId);

                if (rp == null)
                    throw new AppException(AppResponseCode.NOT_FOUND, "Quyền này không tồn tại");

                rp.IsActive = permission.IsActive;
            }

            await _roleRepository.UpdateAsync(role);

        }

        public async Task<IEnumerable<PermissionDomainRequest>> GetRolePermissionsAsync(Guid roleId)
        {
            var role = await _roleRepository.GetByIdAsync(roleId,
                include: i => i.Include(r => r.RolePermissions));

            if (role == null)
                throw new AppException(AppResponseCode.NOT_FOUND);

            var rolePermissions = role.RolePermissions;

            var domains = await _permissionDomainRepository.GetAllAsync(
                include: i => i.Include(rd => rd.Actions));

            var result = domains.Select(d => new PermissionDomainRequest
            {
                DomainName = d.Name,
                Actions = d.Actions.Select(a => new PermissionActionRequest
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
