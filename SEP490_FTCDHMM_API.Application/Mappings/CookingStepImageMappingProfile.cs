using AutoMapper;
using SEP490_FTCDHMM_API.Application.Dtos.RecipeDtos.CookingStep.CookingStepImage;
using SEP490_FTCDHMM_API.Domain.Entities;

namespace SEP490_FTCDHMM_API.Application.Mappings
{
    public class CookingStepImageMappingProfile : Profile
    {
        public CookingStepImageMappingProfile()
        {
            CreateMap<CookingStepImage, CookingStepImageResponse>()
                .ForMember(
                    dest => dest.ImageUrl,
                    opt => opt.MapFrom<UniversalImageUrlResolver<CookingStepImage, CookingStepImageResponse>>()
                );

        }
    }
}
