using SEP490_FTCDHMM_API.Application.Interfaces.Persistence;
using SEP490_FTCDHMM_API.Domain.Entities;
using SEP490_FTCDHMM_API.Infrastructure.Data;

namespace SEP490_FTCDHMM_API.Infrastructure.Repositories
{
    public class RatingRepository : EfRepository<Rating>, IRatingRepository
    {
        public RatingRepository(AppDbContext context) : base(context) { }
    }
}
