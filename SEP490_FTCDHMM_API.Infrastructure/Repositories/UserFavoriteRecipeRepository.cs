using SEP490_FTCDHMM_API.Application.Interfaces.Persistence;
using SEP490_FTCDHMM_API.Domain.Entities;
using SEP490_FTCDHMM_API.Infrastructure.Data;

namespace SEP490_FTCDHMM_API.Infrastructure.Repositories
{
    public class UserFavoriteRecipeRepository : EfRepository<UserFavoriteRecipe>, IUserFavoriteRecipeRepository
    {
        private readonly AppDbContext _context;
        public UserFavoriteRecipeRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }
    }
}
