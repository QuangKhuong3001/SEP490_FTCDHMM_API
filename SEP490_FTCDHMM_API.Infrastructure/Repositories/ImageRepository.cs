using Microsoft.EntityFrameworkCore;
using SEP490_FTCDHMM_API.Application.Interfaces.Persistence;
using SEP490_FTCDHMM_API.Domain.Entities;
using SEP490_FTCDHMM_API.Infrastructure.Data;

namespace SEP490_FTCDHMM_API.Infrastructure.Repositories
{
    public class ImageRepository : EfRepository<Image>, IImageRepository
    {
        private readonly AppDbContext _dbContext;

        public ImageRepository(AppDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<string?> GetAvatarKeyByUserId(Guid userId)
        {
            var user = await _dbContext.Users
                .Where(u => u.Id == userId)
                .Include(u => u.Avatar)
                .FirstOrDefaultAsync();

            if (user == null)
                return null;

            return user.Avatar?.Key;
        }
    }
}