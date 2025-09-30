using Microsoft.AspNetCore.Identity;
using SEP490_FTCDHMM_API.Domain.ValueObjects;

namespace SEP490_FTCDHMM_API.Domain.Entities
{
    public class AppUser : IdentityUser<Guid>
    {
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public Gender Gender { get; set; } = Gender.Other;
        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAtUtc { get; set; }

        public Guid RoleId { get; set; }
        public AppRole Role { get; set; } = null!;
    }
}
