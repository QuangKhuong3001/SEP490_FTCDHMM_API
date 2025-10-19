using SEP490_FTCDHMM_API.Application.Interfaces.Persistence;
using SEP490_FTCDHMM_API.Domain.Entities;
using SEP490_FTCDHMM_API.Infrastructure.Data;

namespace SEP490_FTCDHMM_API.Infrastructure.Repositories
{
    public class LabelRepository : EfRepository<Label>, ILabelRepository
    {
        private readonly AppDbContext _context;
        public LabelRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

    }
}
