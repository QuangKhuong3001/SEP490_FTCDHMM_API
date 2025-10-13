namespace SEP490_FTCDHMM_API.Application.Dtos.AuthDTOs
{
    public class ChangePasswordRequest
    {
        public string CurrentPassword { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;
        public string RePassword { get; set; } = string.Empty;
    }

}
