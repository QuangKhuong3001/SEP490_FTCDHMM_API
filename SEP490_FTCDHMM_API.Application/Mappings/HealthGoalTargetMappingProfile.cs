using AutoMapper;
using SEP490_FTCDHMM_API.Application.Dtos.NutrientTargetDtos;
using SEP490_FTCDHMM_API.Domain.Entities;

namespace SEP490_FTCDHMM_API.Application.Mappings
{
    public class HealthGoalTargetMappingProfile : Profile
    {
        public HealthGoalTargetMappingProfile()
        {
            CreateMap<HealthGoalTarget, NutrientTargetResponse>()
                .ForMember(dest => dest.NutrientId, opt => opt.MapFrom(src => src.NutrientId))
                .ForMember(dest => dest.MinValue, opt => opt.MapFrom(src => src.MinValue))
                .ForMember(dest => dest.MaxValue, opt => opt.MapFrom(src => src.MaxValue))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Nutrient.Name));
        }
    }
}
