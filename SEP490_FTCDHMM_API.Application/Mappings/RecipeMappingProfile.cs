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
                );

            CreateMap<Recipe, MyRecipeResponse>()
                .ForMember(
                    dest => dest.ImageUrl,
                    opt => opt.MapFrom<UniversalImageUrlResolver<Recipe, MyRecipeResponse>>()
                );

            CreateMap<Recipe, RecipeDetailsResponse>()
                .ForMember(
                    dest => dest.ImageUrl,
                    opt => opt.MapFrom<UniversalImageUrlResolver<Recipe, RecipeDetailsResponse>>()
                )
                .ForMember(
                    dest => dest.Author,
                    opt => opt.MapFrom(r => r.Author)
                );
        }
    }
}
