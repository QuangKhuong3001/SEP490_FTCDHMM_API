using AutoMapper;
using APIDtos = SEP490_FTCDHMM_API.Api.Dtos;
using ApplicationDtos = SEP490_FTCDHMM_API.Application.Dtos;

namespace SEP490_FTCDHMM_API.Api.Mappings
{
    public class UserMappingProfile : Profile
    {
        public UserMappingProfile()
        {
            //user manangement
            CreateMap<APIDtos.UserDtos.LockRequest, ApplicationDtos.UserDtos.LockRequest>();
            CreateMap<APIDtos.UserDtos.CreateModeratorAccountRequest, ApplicationDtos.UserDtos.CreateModeratorAccountRequest>();

            //profile
            CreateMap<APIDtos.UserDtos.UpdateProfileRequest, ApplicationDtos.UserDtos.UpdateProfileRequest>();

            CreateMap<APIDtos.UserDtos.UserFilterRequest, ApplicationDtos.UserDtos.UserFilterRequest>();
        }
    }
}
