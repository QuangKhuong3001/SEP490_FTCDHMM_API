using SEP490_FTCDHMM_API.Domain.Entities;

namespace SEP490_FTCDHMM_API.Application.Services.Interfaces.RecipeInterfaces
{
    public interface IRecipeNutritionService
    {
        Task AggregateAsync(Recipe recipe);
    }
}
