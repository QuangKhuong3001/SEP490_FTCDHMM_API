using SEP490_FTCDHMM_API.Domain.Entities;
using SEP490_FTCDHMM_API.Domain.Specifications;

namespace SEP490_FTCDHMM_API.Application.Interfaces.Persistence
{
    public interface IRecipeRepository : IRepository<Recipe>
    {
        Task<IReadOnlyList<Recipe>> GetRecipesRawAsync(RecipeBasicFilterSpec spec);

        Task<List<Recipe>> GetActiveRecentRecipesAsync();
    }
}
