using SEP490_FTCDHMM_API.Application.Dtos.IngredientDtos.USDA;

namespace SEP490_FTCDHMM_API.Application.Interfaces.ExternalServices
{
    public interface IUsdaApiService
    {
        Task<UsdaSearchResult?> SearchAsync(string keyword);
        Task<UsdaFoodDetail?> GetDetailAsync(int fdcId);
    }
}
