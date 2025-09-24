namespace SEP490_FTCDHMM_API.Application.Interfaces
{
    public interface IMailService
    {
        Task SendEmailAsync(string toEmail, string htmlMessage);
    }
}
