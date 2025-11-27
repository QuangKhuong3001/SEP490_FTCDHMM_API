using AutoMapper;
using SEP490_FTCDHMM_API.Application.Dtos.DraftRecipeDtos;
using SEP490_FTCDHMM_API.Application.Dtos.RecipeDtos;
using SEP490_FTCDHMM_API.Application.Dtos.RecipeDtos.RecipeIngredient;
using SEP490_FTCDHMM_API.Domain.Entities;

namespace SEP490_FTCDHMM_API.Application.Mappings
{
    public class RecipeMappingProfile : Profile
    {
        public RecipeMappingProfile()
        {
            CreateMap<Recipe, RecipeResponse>()
                .ForMember(
                    dest => dest.ImageUrl,
                    opt => opt.MapFrom<UniversalImageUrlResolver<Recipe, RecipeResponse>>()
                )
                .ForMember(
                    dest => dest.Author,
                    opt => opt.MapFrom(r => r.Author)
                ).ForMember(
                    dest => dest.Ingredients,
                    opt => opt.MapFrom(r => r.RecipeIngredients)
                ).ForMember(
                    dest => dest.Labels,
                    opt => opt.MapFrom(r => r.Labels)
                );

            CreateMap<RecipeUserTag, DraftRecipeUserTaggedResponse>()
                .ForMember(dest => dest.Id,
                    opt => opt.MapFrom(src => src.TaggedUserId))
                .ForMember(dest => dest.FirstName,
                    opt => opt.MapFrom(src => src.TaggedUser.FirstName))
                .ForMember(dest => dest.LastName,
                    opt => opt.MapFrom(src => src.TaggedUser.LastName));

            CreateMap<Recipe, MyRecipeResponse>()
                .ForMember(
                    dest => dest.ImageUrl,
                    opt => opt.MapFrom<UniversalImageUrlResolver<Recipe, MyRecipeResponse>>()
                ).ForMember(
                    dest => dest.Ingredients,
                    opt => opt.MapFrom(r => r.RecipeIngredients)
                );

            CreateMap<Recipe, RecipeDetailsResponse>()
                .ForMember(
                    dest => dest.ImageUrl,
                    opt => opt.MapFrom<UniversalImageUrlResolver<Recipe, RecipeDetailsResponse>>()
                )
                .ForMember(
                    dest => dest.Author,
                    opt => opt.MapFrom(r => r.Author)
                ).ForMember(
                    dest => dest.Ingredients,
                    opt => opt.MapFrom(r => r.RecipeIngredients)
                ).ForMember(
                    dest => dest.RecipeTaggeds,
                    opt => opt.MapFrom(r => r.RecipeUserTags)
                ).ForMember(
                    dest => dest.CookingSteps,
                    opt => opt.MapFrom(r => r.CookingSteps)
                );

            CreateMap<RecipeIngredient, RecipeIngredientResponse>()
                .ForMember(
                    dest => dest.Name,
                    opt => opt.MapFrom(r => r.Ingredient.Name)
                );

            CreateMap<Recipe, RecipeRatingResponse>()
                .ForMember(
                    dest => dest.AverageRating,
                    opt => opt.MapFrom(src => src.AvgRating)
                );
        }
    }
}
