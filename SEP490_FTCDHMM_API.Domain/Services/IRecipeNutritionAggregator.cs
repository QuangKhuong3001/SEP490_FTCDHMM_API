using SEP490_FTCDHMM_API.Domain.Entities;

namespace SEP490_FTCDHMM_API.Domain.Services
{
    public interface IRecipeNutritionAggregator
    {
        Task AggregateAndSaveAsync(Recipe recipe);
    }
}
