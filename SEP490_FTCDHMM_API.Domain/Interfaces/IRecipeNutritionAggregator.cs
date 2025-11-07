using SEP490_FTCDHMM_API.Domain.Entities;

namespace SEP490_FTCDHMM_API.Domain.Interfaces
{
    public interface IRecipeNutritionAggregator
    {
        Task AggregateAndSaveAsync(Recipe recipe);
    }
}
