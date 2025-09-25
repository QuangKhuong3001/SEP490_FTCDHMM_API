using AutoMapper;
using APIDtos = SEP490_FTCDHMM_API.Api.Dtos;
using ApplicationDtos = SEP490_FTCDHMM_API.Application.Dtos;
internal class AuthMappingProfile : Profile
{
    public AuthMappingProfile()
    {
        CreateMap<APIDtos.AuthDTOs.RegisterDto, ApplicationDtos.AuthDTOs.RegisterDto>();
    }
}
