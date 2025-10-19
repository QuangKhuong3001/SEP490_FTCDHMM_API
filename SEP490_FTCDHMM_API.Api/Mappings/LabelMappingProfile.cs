using AutoMapper;
using APIDtos = SEP490_FTCDHMM_API.Api.Dtos;
using ApplicationDtos = SEP490_FTCDHMM_API.Application.Dtos;

namespace SEP490_FTCDHMM_API.Api.Mappings
{
    public class LabelMappingProfile : Profile
    {
        public LabelMappingProfile()
        {

            //filter
            CreateMap<APIDtos.LabelDtos.LabelFilterRequest, ApplicationDtos.LabelDtos.LabelFilterRequest>();
            CreateMap<APIDtos.LabelDtos.LabelSearchDropboxRequest, ApplicationDtos.LabelDtos.LabelSearchDropboxRequest>();

            //create
            CreateMap<APIDtos.LabelDtos.CreateLabelRequest, ApplicationDtos.LabelDtos.CreateLabelRequest>();

            //update
            CreateMap<APIDtos.LabelDtos.UpdateColorCodeRequest, ApplicationDtos.LabelDtos.UpdateColorCodeRequest>();

        }
    }
}
