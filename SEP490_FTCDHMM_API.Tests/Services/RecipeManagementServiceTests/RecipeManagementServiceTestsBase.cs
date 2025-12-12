using Moq;
using SEP490_FTCDHMM_API.Application.Interfaces.ExternalServices;
using SEP490_FTCDHMM_API.Application.Interfaces.Persistence;
using SEP490_FTCDHMM_API.Application.Interfaces.SystemServices;
using SEP490_FTCDHMM_API.Application.Services.Implementations.RecipeImplementation;

namespace SEP490_FTCDHMM_API.Tests.Services.RecipeManagementServiceTests
{
    public abstract class RecipeManagementServiceTestsBase
    {
        protected readonly Mock<IRecipeRepository> RecipeRepoMock;
        protected readonly Mock<IMailService> MailServiceMock;
        protected readonly Mock<IEmailTemplateService> TemplateServiceMock;

        protected readonly RecipeManagementService Service;

        protected RecipeManagementServiceTestsBase()
        {
            RecipeRepoMock = new Mock<IRecipeRepository>();
            MailServiceMock = new Mock<IMailService>();
            TemplateServiceMock = new Mock<IEmailTemplateService>();

            Service = new RecipeManagementService(
                RecipeRepoMock.Object,
                MailServiceMock.Object,
                TemplateServiceMock.Object
            );
        }
    }
}
