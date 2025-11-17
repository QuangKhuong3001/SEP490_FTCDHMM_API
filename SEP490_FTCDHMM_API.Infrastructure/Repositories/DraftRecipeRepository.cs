using Microsoft.EntityFrameworkCore;
using SEP490_FTCDHMM_API.Application.Interfaces.Persistence;
using SEP490_FTCDHMM_API.Domain.Entities;
using SEP490_FTCDHMM_API.Infrastructure.Data;

namespace SEP490_FTCDHMM_API.Infrastructure.Repositories
{
    public class DraftRecipeRepository : EfRepository<DraftRecipe>, IDraftRecipeRepository
    {
        private readonly AppDbContext _dbContext;

        public DraftRecipeRepository(AppDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<DraftRecipe?> GetDraftByAuthorIdAsync(Guid authorId)
        {
            return await _dbContext.DraftRecipes.Where(d => d.AuthorId == authorId)
                    .Include(r => r.DraftCookingSteps)
                        .ThenInclude(cs => cs.DraftCookingStepImages)
                            .ThenInclude(si => si.Image)
                    .Include(r => r.DraftRecipeUserTags)
                        .ThenInclude(cs => cs.TaggedUser)
                    .FirstOrDefaultAsync();
        }
    }
}
