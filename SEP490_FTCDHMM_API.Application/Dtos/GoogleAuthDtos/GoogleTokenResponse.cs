namespace SEP490_FTCDHMM_API.Application.Dtos.GoogleAuthDtos
{
    public class GoogleTokenResponse
    {
        public string AccessToken { get; set; } = string.Empty;
        public string IdToken { get; set; } = string.Empty;
        public string? RefreshToken { get; set; }
        public int ExpiresIn { get; set; }
        public string TokenType { get; set; } = string.Empty;
    }
}
