using Microsoft.EntityFrameworkCore;
using SEP490_FTCDHMM_API.Application.Interfaces;
using SEP490_FTCDHMM_API.Domain.Entities;
using SEP490_FTCDHMM_API.Infrastructure.Data;

namespace SEP490_FTCDHMM_API.Infrastructure.Repositories
{
    public class RoleRepository : EfRepository<AppRole>, IRoleRepository
    {
        private readonly AppDbContext _dbContext;

        public RoleRepository(AppDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<AppRole?> GetRoleWithPermissionsAsync(Guid roleId)
        {
            return await _dbContext.Roles
                .Include(r => r.RolePermissions)
                    .ThenInclude(rp => rp.PermissionAction)
                        .ThenInclude(pa => pa.PermissionDomain)
                .FirstOrDefaultAsync(r => r.Id == roleId);
        }

        public async Task<AppRole?> FindByNameAsync(string roleName)
        {
            return await _dbContext.Roles.FirstOrDefaultAsync(r => r.Name == roleName);
        }
    }
}
