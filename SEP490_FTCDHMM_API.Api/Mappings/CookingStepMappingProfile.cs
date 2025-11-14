using AutoMapper;

using APIDtos = SEP490_FTCDHMM_API.Api.Dtos;
using ApplicationDtos = SEP490_FTCDHMM_API.Application.Dtos;

namespace SEP490_FTCDHMM_API.Api.Mappings
{
    public class CookingStepMappingProfile : Profile
    {
        public CookingStepMappingProfile()
        {
            CreateMap<APIDtos.CookingStepDtos.CookingStepRequest, ApplicationDtos.CookingStepDtos.CookingStepRequest>();
        }
    }
}