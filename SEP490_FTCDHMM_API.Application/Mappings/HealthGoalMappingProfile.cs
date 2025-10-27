using AutoMapper;
using SEP490_FTCDHMM_API.Application.Dtos.HealthGoalDtos;
using SEP490_FTCDHMM_API.Domain.Entities;

namespace SEP490_FTCDHMM_API.Application.Mappings
{
    public class HealthGoalMappingProfile : Profile
    {
        public HealthGoalMappingProfile()
        {
            CreateMap<HealthGoal, HealthGoalResponse>();
        }
    }
}
