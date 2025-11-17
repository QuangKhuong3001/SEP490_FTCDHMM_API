using AutoMapper;
using SEP490_FTCDHMM_API.Application.Dtos.Common;

namespace SEP490_FTCDHMM_API.Api.Mappings.Converter
{
    public class GlobalFileMappingProfile : Profile
    {
        public GlobalFileMappingProfile()
        {
            CreateMap<IFormFile, FileUploadModel?>()
                .ConvertUsing<FormFileToUploadConverter>();
        }
    }
}
