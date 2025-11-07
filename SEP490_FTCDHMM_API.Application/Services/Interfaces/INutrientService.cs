using SEP490_FTCDHMM_API.Application.Dtos.NutrientDtos;

namespace SEP490_FTCDHMM_API.Application.Services.Interfaces
{
    public interface INutrientService
    {
        Task<IEnumerable<NutrientNameResponse>> GetRequiredNutrientList();
        Task<IEnumerable<NutrientNameResponse>> GetAllNutrient();
    }
}
