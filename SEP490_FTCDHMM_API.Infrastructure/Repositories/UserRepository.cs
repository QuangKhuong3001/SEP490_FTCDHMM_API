using Microsoft.EntityFrameworkCore;
using SEP490_FTCDHMM_API.Application.Interfaces.Persistence;
using SEP490_FTCDHMM_API.Domain.Entities;
using SEP490_FTCDHMM_API.Infrastructure.Data;

namespace SEP490_FTCDHMM_API.Infrastructure.Repositories
{
    public class UserRepository : EfRepository<AppUser>, IUserRepository
    {
        private readonly AppDbContext _dbContext;

        public UserRepository(AppDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<AppUser?> GetByUserNameAsync(string userName)
        {
            return await _dbContext.Users
                .Where(u => u.UserName == userName)
                .FirstOrDefaultAsync();
        }

        public async Task<List<AppUser>> GetTaggableUsersAsync(Guid userId, string? keyword)
        {
            keyword = keyword?.Trim().ToLower();

            var following = _dbContext.UserFollows
                .Where(f => f.FollowerId == userId)
                .Select(f => f.Followee);

            var followers = _dbContext.UserFollows
                .Where(f => f.FolloweeId == userId)
                .Select(f => f.Follower);

            var query = following.Union(followers)
                .Where(u => u.Id != userId)
                .Distinct();

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                query = query.Where(u =>
                    (u.FirstName + " " + u.LastName).ToLower().Contains(keyword));
            }

            return await query.ToListAsync();
        }

    }
}
