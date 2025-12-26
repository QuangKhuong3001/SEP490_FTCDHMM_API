using AutoMapper;
using SEP490_FTCDHMM_API.Application.Dtos.MealDtos;
using SEP490_FTCDHMM_API.Domain.Entities;

namespace SEP490_FTCDHMM_API.Application.Mappings
{
    public class RecommendationMappingProfile : Profile
    {
        public RecommendationMappingProfile()
        {
            CreateMap<UserMealSlot, MealSlotResponse>();
        }
    }
}