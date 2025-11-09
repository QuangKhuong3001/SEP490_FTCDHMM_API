using AutoMapper;
using APIDtos = SEP490_FTCDHMM_API.Api.Dtos;
using ApplicationDtos = SEP490_FTCDHMM_API.Application.Dtos;

namespace SEP490_FTCDHMM_API.Api.Mappings
{
    public class UserIngredientRestrictionMappingProfile : Profile
    {
        public UserIngredientRestrictionMappingProfile()
        {
            CreateMap<APIDtos.UserDietRestriction.CreateIngredientCategoryRestrictionRequest,
                ApplicationDtos.UserDietRestriction.CreateIngredientCategoryRestrictionRequest>();

            CreateMap<APIDtos.UserDietRestriction.CreateIngredientRestrictionRequest,
                ApplicationDtos.UserDietRestriction.CreateIngredientRestrictionRequest>();

            CreateMap<APIDtos.UserDietRestriction.UserDietRestrictionFilterRequest,
                ApplicationDtos.UserDietRestriction.UserDietRestrictionFilterRequest>();
        }
    }
}
