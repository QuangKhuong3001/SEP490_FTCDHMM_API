using AutoMapper;
using SEP490_FTCDHMM_API.Application.Dtos.LabelDtos;
using SEP490_FTCDHMM_API.Domain.Entities;

namespace SEP490_FTCDHMM_API.Application.Mappings
{
    public class LabelMappingProfile : Profile
    {
        public LabelMappingProfile()
        {
            CreateMap<Label, LabelResponse>();
        }
    }
}
