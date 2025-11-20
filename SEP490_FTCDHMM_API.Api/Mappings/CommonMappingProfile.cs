using AutoMapper;
using APIDtos = SEP490_FTCDHMM_API.Api.Dtos;
using ApplicationDtos = SEP490_FTCDHMM_API.Application.Dtos;
namespace SEP490_FTCDHMM_API.Api.Mappings
{
    public class CommonMappingProfile : Profile
    {
        public CommonMappingProfile()
        {
            CreateMap<APIDtos.Common.PaginationParams, ApplicationDtos.Common.PaginationParams>();
            CreateMap<APIDtos.RecipeDtos.RecipePaginationParams, ApplicationDtos.RecipeDtos.RecipePaginationParams>();
        }
    }
}
