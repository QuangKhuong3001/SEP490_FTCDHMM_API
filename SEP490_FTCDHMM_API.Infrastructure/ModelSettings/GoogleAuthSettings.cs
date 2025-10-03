namespace SEP490_FTCDHMM_API.Infrastructure.ModelSettings
{
    public class GoogleAuthSettings
    {
        public string ClientId { get; set; } = string.Empty;
        public string ClientSecret { get; set; } = string.Empty;
        public string RedirectUri { get; set; } = string.Empty;
        public string TokenEndpoint { get; set; } = "https://oauth2.googleapis.com/token";
        public string UserInfoEndpoint { get; set; } = "https://www.googleapis.com/oauth2/v3/userinfo";
        public string PeopleApiEndpoint { get; set; } = "https://www.googleapis.com/oauth2/v3/userinfo";
    }
}
