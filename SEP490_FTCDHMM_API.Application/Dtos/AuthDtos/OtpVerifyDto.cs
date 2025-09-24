namespace SEP490_FTCDHMM_API.Application.Dtos.AuthDTOs
{
    public class OtpVerifyDto
    {
        public string Email { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
    }
}