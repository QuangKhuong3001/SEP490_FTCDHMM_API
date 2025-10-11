using SEP490_FTCDHMM_API.Application.Interfaces;
using SEP490_FTCDHMM_API.Domain.Entities;
using SEP490_FTCDHMM_API.Infrastructure.Data;

namespace SEP490_FTCDHMM_API.Infrastructure.Repositories
{
    public class UserFollowRepository : EfRepository<UserFollow>, IUserFollowRepository
    {
        private readonly AppDbContext _context;
        public UserFollowRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }


    }
}
