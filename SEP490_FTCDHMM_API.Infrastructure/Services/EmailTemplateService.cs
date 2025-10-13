using Microsoft.AspNetCore.Hosting;
using SEP490_FTCDHMM_API.Application.Interfaces.SystemServices;
using SEP490_FTCDHMM_API.Domain.ValueObjects;

namespace SEP490_FTCDHMM_API.Infrastructure.Services
{
    public class EmailTemplateService : IEmailTemplateService
    {
        private readonly IWebHostEnvironment _env;

        public EmailTemplateService(IWebHostEnvironment env)
        {
            _env = env;
        }

        public async Task<string> RenderTemplateAsync(EmailTemplateType templateName, Dictionary<string, string> placeholders)
        {

            var filePath = Path.Combine(_env.ContentRootPath, "EmailTemplates", $"{templateName}.html");
            if (!File.Exists(filePath))
                throw new FileNotFoundException($"Template '{templateName}' not found at {filePath}");

            var content = await File.ReadAllTextAsync(filePath);

            foreach (var placeholder in placeholders)
            {
                content = content.Replace($"{{{{{placeholder.Key}}}}}", placeholder.Value);
            }

            return content;
        }
    }
}
