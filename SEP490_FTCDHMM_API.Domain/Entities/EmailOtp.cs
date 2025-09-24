using SEP490_FTCDHMM_API.Domain.ValueObjects;

namespace SEP490_FTCDHMM_API.Domain.Entities
{
    public class EmailOtp
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public required string Code { get; set; }
        public required OtpPurpose Purpose { get; set; }
        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
        public DateTime ExpiresAtUtc { get; set; }
        public int Attempts { get; set; } = 0;
        public bool IsDisabled { get; set; } = false;

        public required string UserId { get; set; }
        public AppUser User { get; set; } = default!;
    }
}
