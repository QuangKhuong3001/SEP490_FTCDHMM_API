using SEP490_FTCDHMM_API.Application.Dtos.Common;
using SEP490_FTCDHMM_API.Application.Dtos.IngredientDtos;

namespace SEP490_FTCDHMM_API.Application.Services.Interfaces
{
    public interface IIngredientService
    {
        Task<PagedResult<IngredientResponse>> GetList(IngredientFilterRequest dto);
        Task<PagedResult<IngredientResponse>> GetListForManager(IngredientFilterRequest dto);
        Task<IngredientDetailsResponse> GetDetails(Guid ingredientId);
        Task<IngredientDetailsResponse> GetDetailsForManager(Guid ingredientId);
        Task CreateIngredient(CreateIngredientRequest dto);
        Task UpdateIngredient(Guid ingredientId, UpdateIngredientRequest dto);
        Task DeleteIngredient(Guid ingredientId);
        Task<IEnumerable<IngredientNameResponse>> GetFromUsdaSourceAsync(string keyword);

    }
}
