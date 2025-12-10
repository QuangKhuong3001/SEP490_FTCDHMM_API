using SEP490_FTCDHMM_API.Domain.ValueObjects;

namespace SEP490_FTCDHMM_API.Domain.Entities
{
    public class EmailOtp
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Code { get; set; } = string.Empty;
        public OtpPurpose Purpose { get; set; } = OtpPurpose.VerifyAccountEmail;
        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
        public DateTime ExpiresAtUtc { get; set; }
        public int Attempts { get; set; } = 0;
        public bool IsDisabled { get; set; } = false;

        public Guid SentToId { get; set; }
        public AppUser SentTo { get; set; } = default!;
    }
}
