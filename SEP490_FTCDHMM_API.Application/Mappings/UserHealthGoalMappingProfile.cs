using AutoMapper;
using SEP490_FTCDHMM_API.Application.Dtos.HealthGoalDtos;
using SEP490_FTCDHMM_API.Application.Dtos.NutrientDtos.NutrientTarget;
using SEP490_FTCDHMM_API.Domain.Entities;

namespace SEP490_FTCDHMM_API.Application.Mappings
{
    public class UserHealthGoalMappingProfile : Profile
    {
        public UserHealthGoalMappingProfile()
        {
            CreateMap<HealthGoal, UserHealthGoalResponse>()
                .ForMember(dest => dest.HealthGoalId, opt =>
                    opt.MapFrom(src => src.Id));

            CreateMap<HealthGoalTarget, NutrientTargetResponse>()
                .ForMember(dest => dest.Name, opt =>
                    opt.MapFrom(src => src.Nutrient.Name));

            CreateMap<CustomHealthGoal, UserHealthGoalResponse>()
                .ForMember(dest => dest.CustomHealthGoalId, opt =>
                    opt.MapFrom(src => src.Id));

            CreateMap<CustomHealthGoalTarget, NutrientTargetResponse>()
                .ForMember(dest => dest.Name, opt =>
                    opt.MapFrom(src => src.Nutrient.Name));
        }
    }
}
