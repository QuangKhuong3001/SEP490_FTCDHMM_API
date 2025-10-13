namespace SEP490_FTCDHMM_API.Application.Interfaces.ExternalServices
{
    public interface IMailService
    {
        Task SendEmailAsync(string toEmail, string htmlMessage);
    }
}
