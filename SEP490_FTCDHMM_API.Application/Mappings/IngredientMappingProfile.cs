using AutoMapper;
using SEP490_FTCDHMM_API.Application.Dtos.IngredientCategoryDtos;
using SEP490_FTCDHMM_API.Application.Dtos.IngredientDtos;
using SEP490_FTCDHMM_API.Application.Dtos.NutrientDtos;
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
                    opt => opt.MapFrom(src =>
                        src.IngredientCategoryAssignments
                           .Select(a => a.Category)
                           .ToList()));

            CreateMap<Ingredient, IngredientDetailsResponse>()
                .ForMember(dest => dest.Categories,
                    opt => opt.MapFrom(src =>
                        src.IngredientCategoryAssignments.Select(a => a.Category)))
                .ForMember(dest => dest.Nutrients,
                    opt => opt.MapFrom(src => src.IngredientNutrients));

            CreateMap<IngredientCategory, IngredientCategoryResponse>();

            CreateMap<IngredientNutrient, NutrientResponse>()
                .ForMember(dest => dest.Name,
                    opt => opt.MapFrom(src => src.Nutrient.Name))
                .ForMember(dest => dest.Unit,
                    opt => opt.MapFrom(src => src.Nutrient.Unit.Symbol))
                .ForMember(dest => dest.Min, opt => opt.MapFrom(src => src.Min))
                .ForMember(dest => dest.Max, opt => opt.MapFrom(src => src.Max))
                .ForMember(dest => dest.Median, opt => opt.MapFrom(src => src.Median));
        }
    }
}
