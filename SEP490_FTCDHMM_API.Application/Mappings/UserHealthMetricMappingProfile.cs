using AutoMapper;
using SEP490_FTCDHMM_API.Application.Dtos.UserHealthMetricDtos;
using SEP490_FTCDHMM_API.Domain.Entities;

namespace SEP490_FTCDHMM_API.Application.Mappings
{
    public class UserHealthMetricMappingProfile : Profile
    {
        public UserHealthMetricMappingProfile()
        {
            CreateMap<UserHealthMetric, UserHealthMetricResponse>();

        }
    }
}