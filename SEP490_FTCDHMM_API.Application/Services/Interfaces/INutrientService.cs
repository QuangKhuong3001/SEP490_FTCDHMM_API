using SEP490_FTCDHMM_API.Application.Dtos.NutrientDtos;

namespace SEP490_FTCDHMM_API.Application.Services.Interfaces
{
    public interface INutrientService
    {
        Task<List<NutrientNameResponse>> GetRequiredNutrientList();
        Task<List<NutrientNameResponse>> GetAllNutrient();
    }
}
