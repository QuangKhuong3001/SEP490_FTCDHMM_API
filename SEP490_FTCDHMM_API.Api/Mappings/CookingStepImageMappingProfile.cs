using AutoMapper;
using SEP490_FTCDHMM_API.Application.Dtos.Common;
using APIDtos = SEP490_FTCDHMM_API.Api.Dtos;
using ApplicationDtos = SEP490_FTCDHMM_API.Application.Dtos;

namespace SEP490_FTCDHMM_API.Api.Mappings
{
    public class CookingStepImageMappingProfile : Profile
    {
        public CookingStepImageMappingProfile()
        {
            CreateMap<APIDtos.CookingStepImageDtos.CookingStepImageRequest, ApplicationDtos.CookingStepImageDtos.CookingStepImageRequest>()
                .ForMember(dest => dest.Image, opt => opt.MapFrom(src =>
                    new FileUploadModel
                    {
                        FileName = src.Image.FileName,
                        ContentType = src.Image.ContentType,
                        Content = src.Image.OpenReadStream()
                    }
                ));

        }
    }
}