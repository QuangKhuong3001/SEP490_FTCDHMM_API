namespace SEP490_FTCDHMM_API.Application.Dtos.AuthDTOs
{
    public class LoginRequest
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
