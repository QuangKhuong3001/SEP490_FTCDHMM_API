using AutoMapper;
using APIDtos = SEP490_FTCDHMM_API.Api.Dtos;
using ApplicationDtos = SEP490_FTCDHMM_API.Application.Dtos;

namespace SEP490_FTCDHMM_API.Api.Mappings
{
    public class DraftRecipeMappingProfile : Profile
    {
        public DraftRecipeMappingProfile()
        {
            //request
            CreateMap<APIDtos.DraftRecipeDtos.DraftRecipeRequest, ApplicationDtos.DraftRecipeDtos.DraftRecipeRequest>()
                .ForMember(dest => dest.Image, opt => opt.MapFrom(src => src.Image));

            //member of request
            CreateMap<APIDtos.DraftRecipeDtos.DraftRecipeIngredient.DraftRecipeIngredientRequest, ApplicationDtos.DraftRecipeDtos.DraftRecipeIngredient.DraftRecipeIngredientRequest>();
            CreateMap<APIDtos.DraftRecipeDtos.DraftCookingStep.DraftCookingStepRequest, ApplicationDtos.DraftRecipeDtos.DraftCookingStep.DraftCookingStepRequest>();
            CreateMap<APIDtos.DraftRecipeDtos.DraftCookingStep.DraftCookingStepImage.DraftCookingStepImageRequest,
                ApplicationDtos.DraftRecipeDtos.DraftCookingStep.DraftCookingStepImage.DraftCookingStepImageRequest>();

        }
    }
}
