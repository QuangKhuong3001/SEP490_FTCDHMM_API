
using AutoMapper;
using APIDtos = SEP490_FTCDHMM_API.Api.Dtos;
using ApplicationDtos = SEP490_FTCDHMM_API.Application.Dtos;

namespace SEP490_FTCDHMM_API.Api.Mappings
{
    public class UserHealthMetricMappingProfile : Profile
    {
        public UserHealthMetricMappingProfile()
        {
            CreateMap<APIDtos.UserHealthMetricDtos.CreateUserHealthMetricRequest, ApplicationDtos.UserHealthMetricDtos.CreateUserHealthMetricRequest>();
            CreateMap<APIDtos.UserHealthMetricDtos.UpdateUserHealthMetricRequest, ApplicationDtos.UserHealthMetricDtos.UpdateUserHealthMetricRequest>();
        }
    }
}