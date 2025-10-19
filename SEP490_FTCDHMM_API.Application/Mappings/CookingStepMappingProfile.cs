using AutoMapper;
using SEP490_FTCDHMM_API.Application.Dtos.CookingStepDtos;
using SEP490_FTCDHMM_API.Domain.Entities;

namespace SEP490_FTCDHMM_API.Application.Mappings
{
    public class CookingStepMappingProfile : Profile
    {
        public CookingStepMappingProfile()
        {
            CreateMap<CookingStep, CookingStepResponse>()
                .ForMember(
                    dest => dest.ImageUrl,
                    opt => opt.MapFrom<UniversalImageUrlResolver<CookingStep, CookingStepResponse>>()
                );

        }
    }
}
