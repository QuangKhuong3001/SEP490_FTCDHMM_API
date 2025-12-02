using AutoMapper;
using SEP490_FTCDHMM_API.Application.Dtos.ReportDtos;
using SEP490_FTCDHMM_API.Domain.Entities;

namespace SEP490_FTCDHMM_API.Application.Mappings
{
    public class ReportMappingProfile : Profile
    {
        public ReportMappingProfile()
        {
            CreateMap<Report, ReportResponse>()
                .ForMember(dest => dest.TargetType,
                           opt => opt.MapFrom(src => src.TargetType.Value))
                .ForMember(dest => dest.Status,
                           opt => opt.MapFrom(src => src.Status.Value))
                .ForMember(dest => dest.ReporterName,
                           opt => opt.MapFrom(src => $"{src.Reporter.FirstName} {src.Reporter.LastName}"))
                .ForMember(dest => dest.TargetName,
                           opt => opt.Ignore());
        }
    }
}