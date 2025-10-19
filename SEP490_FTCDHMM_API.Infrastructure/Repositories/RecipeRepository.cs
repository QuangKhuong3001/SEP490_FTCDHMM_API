using SEP490_FTCDHMM_API.Application.Interfaces.Persistence;
using SEP490_FTCDHMM_API.Domain.Entities;
using SEP490_FTCDHMM_API.Infrastructure.Data;

namespace SEP490_FTCDHMM_API.Infrastructure.Repositories
{
    public class RecipeRepository : EfRepository<Recipe>, IRecipeRepository
    {
        private readonly AppDbContext _context;
        public RecipeRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

    }
}
