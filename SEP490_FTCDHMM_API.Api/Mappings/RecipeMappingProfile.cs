using AutoMapper;
using SEP490_FTCDHMM_API.Application.Dtos.Common;
using APIDtos = SEP490_FTCDHMM_API.Api.Dtos;
using ApplicationDtos = SEP490_FTCDHMM_API.Application.Dtos;

namespace SEP490_FTCDHMM_API.Api.Mappings
{
    public class RecipeMappingProfile : Profile
    {
        public RecipeMappingProfile()
        {
            CreateMap<Api.Dtos.RecipeDtos.CreateRecipeRequest,
                       Application.Dtos.RecipeDtos.CreateRecipeRequest>()
                .ForMember(dest => dest.Image, opt => opt.MapFrom(src =>
                    src.Image != null
                        ? new FileUploadModel
                        {
                            FileName = src.Image.FileName,
                            Content = src.Image.OpenReadStream(),
                            ContentType = src.Image.ContentType
                        }
                        : null
                ));

            CreateMap<APIDtos.RecipeDtos.RecipeFilterRequest, ApplicationDtos.RecipeDtos.RecipeFilterRequest>();
            CreateMap<APIDtos.RecipeDtos.UpdateRecipeRequest, ApplicationDtos.RecipeDtos.UpdateRecipeRequest>();
            CreateMap<APIDtos.RecipeDtos.RecipeIngredientRequest, ApplicationDtos.RecipeDtos.RecipeIngredientRequest>();
        }
    }
}
