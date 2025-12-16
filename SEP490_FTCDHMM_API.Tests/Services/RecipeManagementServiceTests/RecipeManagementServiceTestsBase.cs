using Moq;
using SEP490_FTCDHMM_API.Application.Interfaces.ExternalServices;
using SEP490_FTCDHMM_API.Application.Interfaces.Persistence;
using SEP490_FTCDHMM_API.Application.Interfaces.SystemServices;
using SEP490_FTCDHMM_API.Application.Services.Implementations.RecipeImplementation;
using SEP490_FTCDHMM_API.Application.Services.Implementations.SEP490_FTCDHMM_API.Application.Interfaces;

namespace SEP490_FTCDHMM_API.Tests.Services.RecipeManagementServiceTests
{
    public abstract class RecipeManagementServiceTestsBase
    {
        protected readonly Mock<IRecipeRepository> RecipeRepoMock;
        protected readonly Mock<IMailService> MailServiceMock;
        protected readonly Mock<IEmailTemplateService> TemplateServiceMock;
        protected readonly Mock<ICacheService> CacheServiceMock;

        protected readonly RecipeManagementService Service;

        protected RecipeManagementServiceTestsBase()
        {
            RecipeRepoMock = new Mock<IRecipeRepository>(MockBehavior.Strict);
            MailServiceMock = new Mock<IMailService>(MockBehavior.Strict);
            TemplateServiceMock = new Mock<IEmailTemplateService>(MockBehavior.Strict);
            CacheServiceMock = new Mock<ICacheService>(MockBehavior.Strict);

            Service = new RecipeManagementService(
                RecipeRepoMock.Object,
                MailServiceMock.Object,
                CacheServiceMock.Object,
                TemplateServiceMock.Object
            );
        }
    }
}
