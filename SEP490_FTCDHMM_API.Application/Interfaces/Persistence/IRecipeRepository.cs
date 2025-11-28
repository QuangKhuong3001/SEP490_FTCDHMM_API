using SEP490_FTCDHMM_API.Domain.Entities;

namespace SEP490_FTCDHMM_API.Application.Interfaces.Persistence
{
    public interface IRecipeRepository : IRepository<Recipe>
    {
        Task<List<Recipe>> GetActiveRecentRecipesAsync();
        Task<List<Recipe>> GetActiveRecentRecipesWithDetailsAsync();
    }
}
