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
            // Map UserHealthGoal to UserHealthGoalResponse
            CreateMap<UserHealthGoal, UserHealthGoalResponse>()
                .ForMember(dest => dest.HealthGoalId, opt =>
                    opt.MapFrom(src => src.HealthGoalId))
                .ForMember(dest => dest.CustomHealthGoalId, opt =>
                    opt.MapFrom(src => src.CustomHealthGoalId))
                .ForMember(dest => dest.Name, opt =>
                    opt.MapFrom(src => src.HealthGoal != null ? src.HealthGoal.Name : (src.CustomHealthGoal != null ? src.CustomHealthGoal.Name : string.Empty)))
                .ForMember(dest => dest.Description, opt =>
                    opt.MapFrom(src => src.HealthGoal != null ? src.HealthGoal.Description : (src.CustomHealthGoal != null ? src.CustomHealthGoal.Description : null)))
                .ForMember(dest => dest.Targets, opt =>
                    opt.MapFrom(src => src.HealthGoal != null
                        ? src.HealthGoal.Targets.Select(t => new NutrientTargetResponse
                        {
                            NutrientId = t.NutrientId,
                            Name = t.Nutrient.Name,
                            TargetType = t.TargetType.ToString(),
                            MinValue = t.MinValue,
                            MaxValue = t.MaxValue,
                            MinEnergyPct = t.MinEnergyPct,
                            MaxEnergyPct = t.MaxEnergyPct,
                            Weight = t.Weight
                        }).ToList()
                        : (src.CustomHealthGoal != null
                            ? src.CustomHealthGoal.Targets.Select(t => new NutrientTargetResponse
                            {
                                NutrientId = t.NutrientId,
                                Name = t.Nutrient.Name,
                                TargetType = t.TargetType.ToString(),
                                MinValue = t.MinValue,
                                MaxValue = t.MaxValue,
                                MinEnergyPct = t.MinEnergyPct,
                                MaxEnergyPct = t.MaxEnergyPct,
                                Weight = t.Weight
                            }).ToList()
                            : new List<NutrientTargetResponse>())));

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
