namespace SEP490_FTCDHMM_API.Infrastructure.Persistence.SeedData
{
    public class AdminAccountSettings
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string FirstName { get; set; } = "System";
        public string LastName { get; set; } = "Admin";
    }
}
