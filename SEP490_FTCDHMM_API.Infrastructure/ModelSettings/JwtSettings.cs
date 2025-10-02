namespace SEP490_FTCDHMM_API.Infrastructure.ModelSettings
{
    public class JwtSettings
    {
        public string Key { get; set; } = string.Empty;
        public string Issuer { get; set; } = string.Empty;
        public string Audience { get; set; } = string.Empty;
        public int ExpiryDays { get; set; } = 1;
    }
}
