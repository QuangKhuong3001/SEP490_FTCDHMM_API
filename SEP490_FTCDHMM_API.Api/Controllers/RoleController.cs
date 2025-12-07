using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SEP490_FTCDHMM_API.Api.Dtos.Common;
using SEP490_FTCDHMM_API.Api.Dtos.RoleDtos;
using SEP490_FTCDHMM_API.Application.Services.Interfaces;
using SEP490_FTCDHMM_API.Domain.Constants;
using ApplicationDtos = SEP490_FTCDHMM_API.Application.Dtos;

namespace SEP490_FTCDHMM_API.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoleController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IRoleService _roleService;

        public RoleController(IMapper mapper, IRoleService roleService)
        {
            _mapper = mapper;
            _roleService = roleService;
        }

        [Authorize(Policy = PermissionPolicies.Role_Create)]
        [HttpPost]
        public async Task<IActionResult> Create(CreateRole dto)
        {
            var appDto = _mapper.Map<ApplicationDtos.RoleDtos.CreateRoleRequest>(dto);

            await _roleService.CreateRole(appDto);
            return Ok();
        }

        [HttpGet]
        [Authorize(Policy = PermissionPolicies.Role_View)]
        public async Task<IActionResult> Get([FromQuery] PaginationParams dto)
        {
            var appDto = _mapper.Map<ApplicationDtos.Common.PaginationParams>(dto);

            var result = await _roleService.GetAllRoles(appDto);
            return Ok(result);
        }

        [HttpPut("{roleId:guid}/active")]
        [Authorize(Policy = PermissionPolicies.Role_Update)]
        public async Task<IActionResult> Active(Guid roleId)
        {
            await _roleService.ActiveRole(roleId);
            return Ok();
        }

        [HttpPut("{roleId:guid}/deactive")]
        [Authorize(Policy = PermissionPolicies.Role_Update)]
        public async Task<IActionResult> Deactive(Guid roleId)
        {
            await _roleService.DeactiveRole(roleId);
            return Ok();
        }

        [HttpPut("{roleId:guid}/permissions")]
        [Authorize(Policy = PermissionPolicies.Role_Update)]
        public async Task<IActionResult> UpdateRolePermissions(Guid roleId, RolePermissionSettingDto dto)
        {
            var appDto = _mapper.Map<ApplicationDtos.RoleDtos.RolePermissionSettingRequest>(dto);

            await _roleService.UpdateRolePermissions(roleId, appDto);
            return Ok();
        }

        [HttpGet("{roleId:guid}/permissions")]
        [Authorize(Policy = PermissionPolicies.Role_View)]
        public async Task<IActionResult> GetRolePermissions(Guid roleId)
        {
            var result = await _roleService.GetRolePermissions(roleId);
            return Ok(result);
        }

    }
}
