namespace SEP490_FTCDHMM_API.Application.Dtos.AuthDTOs
{
    public class RegisterRequest
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Gender { get; set; } = "Male";
        public DateTime DateOfBirth { get; set; }
        public string Password { get; set; } = string.Empty;
        public string RePassword { get; set; } = string.Empty;
    }
}
