using AutoMapper;
using SEP490_FTCDHMM_API.Application.Dtos.IngredientCategoryDtos;
using SEP490_FTCDHMM_API.Domain.Entities;

namespace SEP490_FTCDHMM_API.Application.Mappings
{
    public class IngredientCategoryMappingProfile : Profile
    {
        public IngredientCategoryMappingProfile()
        {
            CreateMap<IngredientCategory, IngredientCategoryResponse>();
        }
    }
}
