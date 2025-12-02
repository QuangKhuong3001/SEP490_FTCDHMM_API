using AutoMapper;
using SEP490_FTCDHMM_API.Application.Dtos.HealthGoalDtos;
using SEP490_FTCDHMM_API.Application.Dtos.NutrientDtos.NutrientTarget;
using SEP490_FTCDHMM_API.Application.Dtos.UserHealthGoalDtos;
using SEP490_FTCDHMM_API.Domain.Entities;

namespace SEP490_FTCDHMM_API.Application.Mappings
{
    public class CustomHealthGoalMappingProfile : Profile
    {
        public CustomHealthGoalMappingProfile()
        {
            CreateMap<CustomHealthGoal, HealthGoalResponse>();

            CreateMap<CustomHealthGoal, UserHealthGoalResponse>()
                .ForMember(dest => dest.HealthGoalId, opt => opt.Ignore())
                .ForMember(dest => dest.CustomHealthGoalId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.StartedAtUtc, opt => opt.Ignore())
                .ForMember(dest => dest.ExpiredAtUtc, opt => opt.Ignore())
                .ForMember(dest => dest.Targets, opt => opt.MapFrom(src => src.Targets));
        }
    }
}
