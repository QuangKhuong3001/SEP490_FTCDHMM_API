using AutoMapper;
using SEP490_FTCDHMM_API.Api.Dtos.RecipeDtos.RecipeIngredient;
using APIDtos = SEP490_FTCDHMM_API.Api.Dtos;
using ApplicationDtos = SEP490_FTCDHMM_API.Application.Dtos;

namespace SEP490_FTCDHMM_API.Api.Mappings
{
    public class RecipeMappingProfile : Profile
    {
        public RecipeMappingProfile()
        {
            CreateMap<APIDtos.RecipeDtos.CreateRecipeRequest, ApplicationDtos.RecipeDtos.CreateRecipeRequest>()
                .ForMember(dest => dest.Image, opt => opt.MapFrom(src => src.Image));

            CreateMap<APIDtos.RecipeDtos.CookingStep.CookingStepRequest, ApplicationDtos.RecipeDtos.CookingStep.CookingStepRequest>();

            CreateMap<APIDtos.RecipeDtos.CookingStep.CookingStepImage.CookingStepImageRequest, ApplicationDtos.RecipeDtos.CookingStep.CookingStepImage.CookingStepImageRequest>()
                .ForMember(dest => dest.Image, opt => opt.MapFrom(src => src.Image));

            CreateMap<APIDtos.RecipeDtos.RecipeFilterRequest, ApplicationDtos.RecipeDtos.RecipeFilterRequest>();

            CreateMap<APIDtos.RecipeDtos.RecipePaginationParams, ApplicationDtos.RecipeDtos.RecipePaginationParams>();

            CreateMap<APIDtos.Common.PaginationParams, ApplicationDtos.Common.PaginationParams>();

            CreateMap<APIDtos.RecipeDtos.UpdateRecipeRequest, ApplicationDtos.RecipeDtos.UpdateRecipeRequest>();

            CreateMap<RecipeIngredientRequest, ApplicationDtos.RecipeDtos.RecipeIngredient.RecipeIngredientRequest>();

            CreateMap<APIDtos.RecipeDtos.CopyRecipeRequest, ApplicationDtos.RecipeDtos.CopyRecipeRequest>();

            CreateMap<APIDtos.RecipeDtos.RecipeManagementReasonRequest, ApplicationDtos.RecipeDtos.RecipeManagementReasonRequest>();
        }
    }
}
