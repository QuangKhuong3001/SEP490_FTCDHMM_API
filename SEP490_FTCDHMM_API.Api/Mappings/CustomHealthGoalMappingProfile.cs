using AutoMapper;
using APIDtos = SEP490_FTCDHMM_API.Api.Dtos;
using ApplicationDtos = SEP490_FTCDHMM_API.Application.Dtos;

namespace SEP490_FTCDHMM_API.Api.Mappings
{
    public class CustomHealthGoalMappingProfile : Profile
    {
        public CustomHealthGoalMappingProfile()
        {

            CreateMap<APIDtos.CustomHealthGoalDtos.CreateCustomHealthGoalRequest, ApplicationDtos.CustomHealthGoalDtos.CreateCustomHealthGoalRequest>();
            CreateMap<APIDtos.CustomHealthGoalDtos.UpdateCustomHealthGoalRequest, ApplicationDtos.CustomHealthGoalDtos.UpdateCustomHealthGoalRequest>();

        }
    }
}