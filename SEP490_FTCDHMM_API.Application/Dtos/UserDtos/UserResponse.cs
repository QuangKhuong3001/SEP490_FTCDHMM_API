using SEP490_FTCDHMM_API.Domain.ValueObjects;

namespace SEP490_FTCDHMM_API.Application.Dtos.UserDtos
{
    public class UserResponse
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public DateTime CreatedAtUTC { get; set; }
        public string Status { get; set; } = UserStatus.Unverified;
        public string? LockReason { get; set; }
        public DateTimeOffset? LockoutEnd { get; set; }
        public string UserName { get; set; } = string.Empty;
    }
}
