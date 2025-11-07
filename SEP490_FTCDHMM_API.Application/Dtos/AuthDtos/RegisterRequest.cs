namespace SEP490_FTCDHMM_API.Application.Dtos.AuthDTOs
{
    public class RegisterDto
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Gender { get; set; } = "Other";
        public DateTime DateOfBirth { get; set; }
        public string Password { get; set; } = string.Empty;
        public string RePassword { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;

    }
}
