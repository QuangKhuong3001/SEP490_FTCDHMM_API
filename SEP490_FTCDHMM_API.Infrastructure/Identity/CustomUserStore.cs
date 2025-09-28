using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SEP490_FTCDHMM_API.Domain.Entities;
using SEP490_FTCDHMM_API.Infrastructure.Data;

namespace SEP490_FTCDHMM_API.Infrastructure.Identity
{
    public class CustomUserStore : UserStore<AppUser, AppRole, AppDbContext, string>
    {
        public CustomUserStore(AppDbContext context, IdentityErrorDescriber? describer = null)
            : base(context, describer)
        {
        }

        public override async Task<IList<string>> GetRolesAsync(AppUser user,
            CancellationToken cancellationToken = default)
        {
            ThrowIfDisposed();
            if (user == null) throw new ArgumentNullException(nameof(user));

            // Use custom role relationship instead of AspNetUserRoles
            var role = await Context.Set<AppRole>()
                .Where(r => r.Id == user.RoleId)
                .Select(r => r.Name)
                .FirstOrDefaultAsync(cancellationToken);

            return role != null ? new List<string> { role } : new List<string>();
        }

        public override async Task<bool> IsInRoleAsync(AppUser user, string normalizedRoleName,
            CancellationToken cancellationToken = default)
        {
            ThrowIfDisposed();
            if (user == null) throw new ArgumentNullException(nameof(user));
            if (string.IsNullOrEmpty(normalizedRoleName))
                throw new ArgumentException("Value cannot be null or empty.", nameof(normalizedRoleName));

            // Use custom role relationship
            var role = await Context.Set<AppRole>()
                .Where(r => r.Id == user.RoleId && r.NormalizedName == normalizedRoleName)
                .FirstOrDefaultAsync(cancellationToken);

            return role != null;
        }

        public override async Task AddToRoleAsync(AppUser user, string normalizedRoleName,
            CancellationToken cancellationToken = default)
        {
            ThrowIfDisposed();
            if (user == null) throw new ArgumentNullException(nameof(user));
            if (string.IsNullOrEmpty(normalizedRoleName))
                throw new ArgumentException("Value cannot be null or empty.", nameof(normalizedRoleName));

            // Update custom role relationship
            var role = await Context.Set<AppRole>()
                .Where(r => r.NormalizedName == normalizedRoleName)
                .FirstOrDefaultAsync(cancellationToken);

            if (role != null)
            {
                user.RoleId = role.Id;
            }
        }

        public override async Task RemoveFromRoleAsync(AppUser user, string normalizedRoleName,
            CancellationToken cancellationToken = default)
        {
            ThrowIfDisposed();
            if (user == null) throw new ArgumentNullException(nameof(user));
            if (string.IsNullOrEmpty(normalizedRoleName))
                throw new ArgumentException("Value cannot be null or empty.", nameof(normalizedRoleName));

            // Check if user has this role via custom relationship
            var hasRole = await IsInRoleAsync(user, normalizedRoleName, cancellationToken);
            if (hasRole)
            {
                // Set to null or default customer role
                var customerRole = await Context.Set<AppRole>()
                    .Where(r => r.NormalizedName == "CUSTOMER")
                    .FirstOrDefaultAsync(cancellationToken);

                user.RoleId = customerRole?.Id ?? throw new InvalidOperationException("Customer role not found");
            }
        }

        public override async Task<IList<AppUser>> GetUsersInRoleAsync(string normalizedRoleName,
            CancellationToken cancellationToken = default)
        {
            ThrowIfDisposed();
            if (string.IsNullOrEmpty(normalizedRoleName))
                throw new ArgumentException("Value cannot be null or empty.", nameof(normalizedRoleName));

            // Use custom role relationship
            return await (from user in Context.Set<AppUser>()
                         join role in Context.Set<AppRole>() on user.RoleId equals role.Id
                         where role.NormalizedName == normalizedRoleName
                         select user).ToListAsync(cancellationToken);
        }
    }
}
