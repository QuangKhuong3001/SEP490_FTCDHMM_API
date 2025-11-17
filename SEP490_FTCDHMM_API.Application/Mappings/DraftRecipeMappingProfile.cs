using AutoMapper;
using SEP490_FTCDHMM_API.Application.Dtos.DraftRecipeDtos;
using SEP490_FTCDHMM_API.Application.Dtos.DraftRecipeDtos.DraftCookingStep;
using SEP490_FTCDHMM_API.Application.Dtos.DraftRecipeDtos.DraftCookingStep.DraftCookingStepImage;
using SEP490_FTCDHMM_API.Application.Dtos.DraftRecipeDtos.DraftRecipeIngredient;
using SEP490_FTCDHMM_API.Domain.Entities;

namespace SEP490_FTCDHMM_API.Application.Mappings
{
    public class DraftRecipeMappingProfile : Profile
    {
        public DraftRecipeMappingProfile()
        {
            //request
            CreateMap<DraftRecipe, DraftRecipeResponse>()
                .ForMember(
                    dest => dest.ImageUrl,
                    opt => opt.MapFrom<UniversalImageUrlResolver<DraftRecipe, DraftRecipeResponse>>()
                )
                .ForMember(
                    dest => dest.Ingredients,
                    opt => opt.MapFrom(r => r.DraftRecipeIngredients)
                ).ForMember(
                    dest => dest.Labels,
                    opt => opt.MapFrom(r => r.Labels)
                ).ForMember(
                    dest => dest.TaggedUser,
                    opt => opt.MapFrom(r => r.DraftRecipeUserTags)
                ).ForMember(
                    dest => dest.CookingSteps,
                    opt => opt.MapFrom(r => r.DraftCookingSteps)
                );

            //member
            CreateMap<DraftRecipeIngredient, DraftRecipeIngredientResponse>()
                .ForMember(
                    dest => dest.Name,
                    opt => opt.MapFrom(r => r.Ingredient.Name)
                );

            CreateMap<DraftCookingStep, DraftCookingStepResponse>()
                            .ForMember(dest => dest.CookingStepImages,
                                opt => opt.MapFrom(src => src.DraftCookingStepImages));

            CreateMap<DraftCookingStepImage, DraftCookingStepImageResponse>()
                .ForMember(
                    dest => dest.ImageUrl,
                    opt => opt.MapFrom<UniversalImageUrlResolver<DraftCookingStepImage, DraftCookingStepImageResponse>>()
                );

            CreateMap<DraftRecipeUserTag, DraftRecipeUserTaggedResponse>()
                            .ForMember(dest => dest.Id,
                                opt => opt.MapFrom(src => src.TaggedUserId))
                            .ForMember(dest => dest.FirstName,
                                opt => opt.MapFrom(src => src.TaggedUser.FirstName))
                            .ForMember(dest => dest.LastName,
                                opt => opt.MapFrom(src => src.TaggedUser.LastName));
        }
    }
}
