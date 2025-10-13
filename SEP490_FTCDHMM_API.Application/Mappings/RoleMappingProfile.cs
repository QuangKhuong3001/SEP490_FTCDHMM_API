using AutoMapper;
using SEP490_FTCDHMM_API.Application.Dtos.RoleDtos;
using SEP490_FTCDHMM_API.Domain.Entities;

namespace SEP490_FTCDHMM_API.Application.Mappings
{
    public class RoleMappingProfile : Profile
    {
        public RoleMappingProfile()
        {
            CreateMap<AppRole, RoleResponse>();
        }
    }
}