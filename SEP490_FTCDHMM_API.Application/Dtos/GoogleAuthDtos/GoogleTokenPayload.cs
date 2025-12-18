namespace SEP490_FTCDHMM_API.Application.Dtos.GoogleAuthDtos
{
    public class GoogleTokenPayload
    {
        public string Email { get; set; } = string.Empty;
        public bool EmailVerified { get; set; }
        public string? Subject { get; set; }
        public string? Name { get; set; }
        public string? GivenName { get; set; }
        public string? FamilyName { get; set; }
        public string? PictureUrl { get; set; }
        public string? Gender { get; set; }
        public DateTime? Birthday { get; set; }
    }
}
