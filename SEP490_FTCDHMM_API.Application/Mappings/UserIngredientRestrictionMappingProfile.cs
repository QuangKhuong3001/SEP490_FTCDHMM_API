using AutoMapper;
using SEP490_FTCDHMM_API.Application.Dtos.UserDietRestriction;
using SEP490_FTCDHMM_API.Domain.Entities;

namespace SEP490_FTCDHMM_API.Application.Mappings
{
    public class UserIngredientRestrictionMappingProfile : Profile
    {
        public UserIngredientRestrictionMappingProfile()
        {
            CreateMap<UserDietRestriction, UserDietRestrictionResponse>()
                .ForMember(dest => dest.IngredientName,
                           opt => opt.MapFrom(src => src.Ingredient != null ? src.Ingredient.Name : null))
                .ForMember(dest => dest.IngredientCategoryName,
                           opt => opt.MapFrom(src => src.IngredientCategory != null ? src.IngredientCategory.Name : null));
        }
    }
}
