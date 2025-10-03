namespace SEP490_FTCDHMM_API.Application.Dtos.GoogleAuthDtos
{
    public class GoogleCodeLoginRequest
    {
        public string Code { get; set; } = string.Empty;
        public string CodeVerifier { get; set; } = string.Empty;
    }
}
