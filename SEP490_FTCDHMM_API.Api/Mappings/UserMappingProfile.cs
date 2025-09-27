using AutoMapper;
using APIDtos = SEP490_FTCDHMM_API.Api.Dtos;
using ApplicationDtos = SEP490_FTCDHMM_API.Application.Dtos;

namespace SEP490_FTCDHMM_API.Api.Mappings
{
    public class UserMappingProfile : Profile
    {
        public UserMappingProfile()
        {
            CreateMap<APIDtos.UserDtos.LockRequestDto, ApplicationDtos.UserDtos.LockRequestDto>();
            CreateMap<APIDtos.UserDtos.UnlockRequestDto, ApplicationDtos.UserDtos.UnlockRequestDto>();
            CreateMap<APIDtos.UserDtos.CreateModeratorAccountDto, ApplicationDtos.UserDtos.CreateModeratorAccountDto>();
        }
    }
}
