using SEP490_FTCDHMM_API.Application.Interfaces.Persistence;
using SEP490_FTCDHMM_API.Domain.Entities;
using SEP490_FTCDHMM_API.Infrastructure.Data;

namespace SEP490_FTCDHMM_API.Infrastructure.Repositories
{
    public class UserDietRestrictionRepository : EfRepository<UserDietRestriction>, IUserDietRestrictionRepository
    {
        private readonly AppDbContext _dbContext;

        public UserDietRestrictionRepository(AppDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }
    }
}
