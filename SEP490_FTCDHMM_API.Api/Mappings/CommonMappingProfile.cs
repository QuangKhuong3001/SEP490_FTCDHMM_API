using AutoMapper;
using APIDtos = SEP490_FTCDHMM_API.Api.Dtos;
using ApplicationDtos = SEP490_FTCDHMM_API.Application.Dtos;
internal class CommonMappingProfile : Profile
{
    public CommonMappingProfile()
    {
        CreateMap<APIDtos.Common.PaginationParams, ApplicationDtos.Common.PaginationParams>();
    }
}
