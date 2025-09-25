namespace SEP490_FTCDHMM_API.Domain.ValueObjects
{
    public record UserStatus(string Value)
    {
        public const string Verified = "Verified";
        public const string Unverified = "Unverified";
        public const string Locked = "Locked";
    }
}
