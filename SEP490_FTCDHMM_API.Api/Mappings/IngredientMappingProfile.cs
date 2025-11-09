using AutoMapper;
using SEP490_FTCDHMM_API.Api.Dtos.UserDietRestriction;
using APIDtos = SEP490_FTCDHMM_API.Api.Dtos;
using ApplicationDtos = SEP490_FTCDHMM_API.Application.Dtos;
namespace SEP490_FTCDHMM_API.Api.Mappings
{
    public class IngredientMappingProfile : Profile
    {
        public IngredientMappingProfile()
        {
            //create
            CreateMap<APIDtos.IngredientDtos.CreateIngredientRequest, ApplicationDtos.IngredientDtos.CreateIngredientRequest>();

            //update
            CreateMap<APIDtos.IngredientDtos.UpdateIngredientRequest, ApplicationDtos.IngredientDtos.UpdateIngredientRequest>();

            //filter
            CreateMap<UserDietRestrictionFilterRequest, ApplicationDtos.IngredientDtos.IngredientFilterRequest>();

            //detection
            CreateMap<APIDtos.IngredientDetectionDtos.IngredientDetectionUploadRequest, ApplicationDtos.IngredientDetectionDtos.IngredientDetectionUploadRequest>();
        }
    }
}