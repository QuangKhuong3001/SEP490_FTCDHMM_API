using AutoMapper;

using APIDtos = SEP490_FTCDHMM_API.Api.Dtos;
using ApplicationDtos = SEP490_FTCDHMM_API.Application.Dtos;

namespace SEP490_FTCDHMM_API.Api.Mappings
{
    public class IngredientCategoryMappingProfile : Profile
    {
        public IngredientCategoryMappingProfile()
        {

            //create
            CreateMap<APIDtos.IngredientCategoryDtos.CreateIngredientCategoryRequest, ApplicationDtos.IngredientCategoryDtos.CreateIngredientCategoryRequest>();

            //filter
            CreateMap<APIDtos.IngredientCategoryDtos.IngredientCategoryFilterRequest, ApplicationDtos.IngredientCategoryDtos.IngredientCategoryFilterRequest>();
            CreateMap<APIDtos.IngredientCategoryDtos.IngredientCategorySearchDropboxRequest, ApplicationDtos.IngredientCategoryDtos.IngredientCategorySearchDropboxRequest>();

        }
    }
}