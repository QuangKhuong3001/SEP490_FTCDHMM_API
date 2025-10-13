using SEP490_FTCDHMM_API.Application.Dtos.Common;
using SEP490_FTCDHMM_API.Application.Dtos.IngredientDtos;

namespace SEP490_FTCDHMM_API.Application.Services.Interfaces
{
    public interface IIngredientService
    {
        Task<PagedResult<IngredientResponse>> GetList(IngredientFilterRequest dto);
        Task<IngredientDetailsResponse> GetDetails(Guid ingredientId);
        Task CreateIngredient(CreateIngredientRequest dto);
        Task UpdateIngredient(Guid ingredientId, UpdateIngredientRequest dto, CancellationToken ct);
        Task DeleteIngredient(Guid ingredientId);
        Task<List<IngredientNameResponse>> GetTop5Async(string keyword, CancellationToken ct = default);

    }
}
