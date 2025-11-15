using Microsoft.AspNetCore.Http;

namespace SEP490_FTCDHMM_API.Application.Dtos.UserDtos
{
    public class UpdateProfileRequest
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Gender { get; set; } = string.Empty;
        public IFormFile? Avatar { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string? Address { get; set; }
        public string? Bio { get; set; }
    }
}
