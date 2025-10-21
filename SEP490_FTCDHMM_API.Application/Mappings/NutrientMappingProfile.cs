﻿using AutoMapper;
using SEP490_FTCDHMM_API.Application.Dtos.NutrientDtos;
using SEP490_FTCDHMM_API.Domain.Entities;

namespace SEP490_FTCDHMM_API.Application.Mappings
{
    public class NutrientMappingProfile : Profile
    {
        public NutrientMappingProfile()
        {
            CreateMap<IngredientNutrient, NutrientResponse>()
                .ForMember(dest => dest.Name, opt =>
                    opt.MapFrom(src => src.Nutrient.Name))
                .ForMember(dest => dest.Unit,
                    opt => opt.MapFrom(src => src.Nutrient.Unit.Symbol));

            CreateMap<Nutrient, NutrientNameResponse>()
                .ForMember(dest => dest.Unit,
                    opt => opt.MapFrom(src => src.Unit.Symbol));
        }
    }
}
