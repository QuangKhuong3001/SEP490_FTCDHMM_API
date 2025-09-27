using AutoMapper;
using APIDtos = SEP490_FTCDHMM_API.Api.Dtos;
using ApplicationDtos = SEP490_FTCDHMM_API.Application.Dtos;
namespace SEP490_FTCDHMM_API.Api.Mappings
{
    public class AuthMappingProfile : Profile
    {
        public AuthMappingProfile()
        {
            CreateMap<APIDtos.AuthDTOs.RegisterDto, ApplicationDtos.AuthDTOs.RegisterDto>();
            CreateMap<APIDtos.AuthDTOs.LoginDto, ApplicationDtos.AuthDTOs.LoginDto>();
        }
    }
}


