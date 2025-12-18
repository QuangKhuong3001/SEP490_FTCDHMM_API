namespace SEP490_FTCDHMM_API.Application.Dtos.GoogleAuthDtos
{
    public class GoogleUserInfo
    {
        public string Email { get; set; } = string.Empty;
        public string? Name { get; set; }
        public string? GivenName { get; set; }
        public string? FamilyName { get; set; }
        public string? PictureUrl { get; set; }
        public string? Gender { get; set; }
        public DateTime? Birthday { get; set; }
    }
}
