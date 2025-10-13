namespace SEP490_FTCDHMM_API.Application.Dtos.AuthDTOs
{
    public class ResetPasswordWithTokenDto
    {
        public string Email { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;
        public string RePassword { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
    }
}
