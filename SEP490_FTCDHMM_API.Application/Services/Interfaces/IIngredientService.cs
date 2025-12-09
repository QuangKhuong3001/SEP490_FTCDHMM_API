using SEP490_FTCDHMM_API.Application.Dtos.Common;
using SEP490_FTCDHMM_API.Application.Dtos.IngredientDtos;

namespace SEP490_FTCDHMM_API.Application.Services.Interfaces
{
    public interface IIngredientService
    {
        Task<PagedResult<IngredientResponse>> GetIngredientsAsync(IngredientFilterRequest dto);
        Task<PagedResult<IngredientResponse>> GetIngredientsForManagerAsync(IngredientFilterRequest dto);
        Task<IngredientDetailsResponse> GetIngredientDetailsAsync(Guid ingredientId);
        Task<IngredientDetailsResponse> GetIngredientDetailsForManagerAsync(Guid ingredientId);
        Task CreateIngredientAsync(CreateIngredientRequest dto);
        Task UpdateIngredientAsync(Guid ingredientId, UpdateIngredientRequest dto);
        Task DeleteIngredientAsync(Guid ingredientId);
        Task<IEnumerable<IngredientNameResponse>> GetFromUsdaSourceAsync(string keyword);

    }
}
