namespace SEP490_FTCDHMM_API.Application.Dtos.GoogleAuthDtos
{
    public class GoogleProvisionRequest
    {
        public GoogleTokenPayload Payload { get; set; } = new();
        public GoogleUserInfo? UserInfo { get; set; }
        public string? GoogleRefreshToken { get; set; }
    }
}
