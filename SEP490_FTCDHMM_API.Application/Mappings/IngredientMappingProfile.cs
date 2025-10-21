using AutoMapper;
using SEP490_FTCDHMM_API.Application.Dtos.IngredientDtos;
using SEP490_FTCDHMM_API.Domain.Entities;

namespace SEP490_FTCDHMM_API.Application.Mappings
{
    public class IngredientMappingProfile : Profile
    {
        public IngredientMappingProfile()
        {
            CreateMap<Ingredient, IngredientNameResponse>();

            CreateMap<Ingredient, IngredientResponse>()
                .ForMember(dest => dest.CategoryNames,
                    opt => opt.MapFrom(src => src.Categories));

            CreateMap<Ingredient, IngredientDetailsResponse>()
                .ForMember(dest => dest.Nutrients,
                    opt => opt.MapFrom(src => src.IngredientNutrients));
        }
    }
}
