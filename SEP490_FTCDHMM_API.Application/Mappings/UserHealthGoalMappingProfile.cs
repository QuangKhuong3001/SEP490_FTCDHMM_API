using AutoMapper;
using SEP490_FTCDHMM_API.Application.Dtos.NutrientDtos.NutrientTarget;
using SEP490_FTCDHMM_API.Application.Dtos.UserHealthGoalDtos;
using SEP490_FTCDHMM_API.Domain.Entities;

namespace SEP490_FTCDHMM_API.Application.Mappings
{
    public class UserHealthGoalMappingProfile : Profile
    {
        public UserHealthGoalMappingProfile()
        {
            CreateMap<UserHealthGoal, UserHealthGoalResponse>()
                .ForMember(dest => dest.Name, opt =>
                    opt.MapFrom((src, dest) =>
                        src.HealthGoal != null
                            ? src.HealthGoal.Name
                            : src.CustomHealthGoal?.Name ?? string.Empty))
                .ForMember(dest => dest.Description, opt =>
                    opt.MapFrom((src, dest) =>
                        src.HealthGoal != null
                            ? src.HealthGoal.Description
                            : src.CustomHealthGoal?.Description))
                .ForMember(dest => dest.ExpiredAtUtc, opt =>
                    opt.MapFrom(src => src.ExpiredAtUtc))
                .ForMember(dest => dest.Targets, opt =>
                    opt.MapFrom((src, dest) =>
                        src.HealthGoal != null
                            ? src.HealthGoal.Targets
                            : src.CustomHealthGoal!.Targets));

            CreateMap<HealthGoalTarget, NutrientTargetResponse>()
                .ForMember(dest => dest.Name, opt =>
                    opt.MapFrom(src => src.Nutrient.Name));
        }
    }
}
