namespace SEP490_FTCDHMM_API.Application.Interfaces
{
    public interface IEmailTemplateService
    {
        Task<string> RenderTemplateAsync(string templateName, Dictionary<string, string> placeholders);
    }
}
