using SEP490_FTCDHMM_API.Domain.ValueObjects;

namespace SEP490_FTCDHMM_API.Application.Interfaces
{
    public interface IEmailTemplateService
    {
        Task<string> RenderTemplateAsync(EmailTemplateType templateName, Dictionary<string, string> placeholders);
    }
}
