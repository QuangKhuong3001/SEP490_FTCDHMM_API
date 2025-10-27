using AutoMapper;
using SEP490_FTCDHMM_API.Application.Dtos.RecipeDtos;
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
                );

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
                );

            CreateMap<RecipeIngredient, RecipeIngredientResponse>()
                .ForMember(
                    dest => dest.Name,
                    opt => opt.MapFrom(r => r.Ingredient.Name)
                );
        }
    }
}
