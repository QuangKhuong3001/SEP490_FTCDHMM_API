using AutoMapper;
using APIDtos = SEP490_FTCDHMM_API.Api.Dtos;
using ApplicationDtos = SEP490_FTCDHMM_API.Application.Dtos;

namespace SEP490_FTCDHMM_API.Api.Mappings
{
    public class HealthGoalMappingProfile : Profile
    {
        public HealthGoalMappingProfile()
        {

            CreateMap<APIDtos.HealthGoalDtos.CreateHealthGoalRequest, ApplicationDtos.HealthGoalDtos.CreateHealthGoalRequest>();
            CreateMap<APIDtos.HealthGoalDtos.UpdateHealthGoalRequest, ApplicationDtos.HealthGoalDtos.UpdateHealthGoalRequest>();

        }
    }
}