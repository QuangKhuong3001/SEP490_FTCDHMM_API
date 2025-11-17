using AutoMapper;
using APIDtos = SEP490_FTCDHMM_API.Api.Dtos;
using ApplicationDtos = SEP490_FTCDHMM_API.Application.Dtos;

namespace SEP490_FTCDHMM_API.Api.Mappings
{
    public class UserFavoriteRecipeMappingProfile : Profile
    {
        public UserFavoriteRecipeMappingProfile()
        {
            CreateMap<APIDtos.RecipeDtos.UserFavoriteRecipe.FavoriteRecipeFilterRequest, ApplicationDtos.RecipeDtos.UserFavoriteRecipe.FavoriteRecipeFilterRequest>();
        }
    }
}
