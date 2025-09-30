using AutoMapper;
using APIDtos = SEP490_FTCDHMM_API.Api.Dtos;
using ApplicationDtos = SEP490_FTCDHMM_API.Application.Dtos;
namespace SEP490_FTCDHMM_API.Api.Mappings
{
    public class RoleMappingProfile : Profile
    {
        public RoleMappingProfile()
        {
            //create
            CreateMap<APIDtos.RoleDtos.CreateRole, ApplicationDtos.RoleDtos.CreateRoleDto>();

            //PermissionSetting
            CreateMap<APIDtos.RoleDtos.PermissionToggleDto, ApplicationDtos.RoleDtos.PermissionToggleDto>();
            CreateMap<APIDtos.RoleDtos.RolePermissionSettingDto, ApplicationDtos.RoleDtos.RolePermissionSettingDto>();
        }
    }
}