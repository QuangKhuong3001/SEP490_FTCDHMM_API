using SEP490_FTCDHMM_API.Application.Interfaces.Persistence;
using SEP490_FTCDHMM_API.Domain.Entities;
using SEP490_FTCDHMM_API.Infrastructure.Data;

namespace SEP490_FTCDHMM_API.Infrastructure.Repositories
{
    public class RecipeIngredientRepository : EfRepository<RecipeIngredient>, IRecipeIngredientRepository
    {
        public RecipeIngredientRepository(AppDbContext context) : base(context)
        {

        }
    }
}
