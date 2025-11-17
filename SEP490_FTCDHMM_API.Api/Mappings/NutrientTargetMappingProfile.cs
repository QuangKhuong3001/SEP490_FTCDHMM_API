using AutoMapper;
using APIDtos = SEP490_FTCDHMM_API.Api.Dtos;
using ApplicationDtos = SEP490_FTCDHMM_API.Application.Dtos;

namespace SEP490_FTCDHMM_API.Api.Mappings
{
    public class NutrientTargetMappingProfile : Profile
    {
        public NutrientTargetMappingProfile()
        {
            //nutrient
            CreateMap<APIDtos.NutrientDtos.NutrientTarget.NutrientTargetRequest, ApplicationDtos.NutrientDtos.NutrientTarget.NutrientTargetRequest>();
        }
    }
}
