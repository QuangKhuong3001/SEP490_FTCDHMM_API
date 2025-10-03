namespace SEP490_FTCDHMM_API.Api.Dtos.GoogleAuthDtos
{
    public class GoogleCodeLoginRequest
    {
        public string Code { get; set; } = string.Empty;
        public string CodeVerifier { get; set; } = string.Empty;
    }
}
