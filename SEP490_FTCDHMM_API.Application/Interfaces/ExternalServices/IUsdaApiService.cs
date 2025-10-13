using SEP490_FTCDHMM_API.Application.Dtos.IngredientDtos;
namespace SEP490_FTCDHMM_API.Application.Interfaces.ExternalServices
{
    public interface IUsdaApiService
    {
        Task<List<IngredientNameResponse>> GetAsync(string keyword, int take = 5, CancellationToken ct = default);
    }
}
