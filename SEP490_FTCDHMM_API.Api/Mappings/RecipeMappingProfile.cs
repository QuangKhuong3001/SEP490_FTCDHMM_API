using AutoMapper;
using APIDtos = SEP490_FTCDHMM_API.Api.Dtos;
using ApplicationDtos = SEP490_FTCDHMM_API.Application.Dtos;

namespace SEP490_FTCDHMM_API.Api.Mappings
{
    public class RecipeMappingProfile : Profile
    {
        public RecipeMappingProfile()
        {
            CreateMap<APIDtos.RecipeDtos.CreateRecipeRequest, ApplicationDtos.RecipeDtos.CreateRecipeRequest>();
            CreateMap<APIDtos.RecipeDtos.RecipeFilterRequest, ApplicationDtos.RecipeDtos.RecipeFilterRequest>();
        }
    }
}
