using AutoMapper;
using SEP490_FTCDHMM_API.Application.Dtos.NutrientDtos;
using SEP490_FTCDHMM_API.Application.Interfaces.Persistence;
using SEP490_FTCDHMM_API.Application.Services.Interfaces;

namespace SEP490_FTCDHMM_API.Application.Services.Implementations
{
    public class NutrientService : INutrientService
    {
        private readonly IMapper _mapper;
        private readonly INutrientRepository _nutrientRepository;

        public NutrientService(IMapper mapper, INutrientRepository nutrientRepository)
        {
            _mapper = mapper;
            _nutrientRepository = nutrientRepository;
        }

        public async Task<List<NutrientNameResponse>> GetAllNutrient()
        {
            var nutrients = await _nutrientRepository.GetAllAsync(i => i.Unit);
            var result = _mapper.Map<List<NutrientNameResponse>>(nutrients);

            return result;
        }

        public async Task<List<NutrientNameResponse>> GetRequiredNutrientList()
        {
            var requireds = await _nutrientRepository.GetAllAsync(r => r.IsRequired, i => i.Unit);
            var result = _mapper.Map<List<NutrientNameResponse>>(requireds);

            return result;
        }
    }
}
