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
        protected Mock<IRecipeRepository> RecipeRepoMock { get; }
        protected Mock<IMailService> MailServiceMock { get; }
        protected Mock<ICacheService> CacheServiceMock { get; }
        protected Mock<INotificationRepository> NotificationRepositoryMock { get; }
        protected Mock<IRealtimeNotifier> RealtimeNotifierMock { get; }
        protected Mock<IEmailTemplateService> TemplateServiceMock { get; }

        protected RecipeManagementService Service { get; }

        protected RecipeManagementServiceTestsBase()
        {
            RecipeRepoMock = new(MockBehavior.Strict);
            MailServiceMock = new(MockBehavior.Strict);
            CacheServiceMock = new(MockBehavior.Strict);
            NotificationRepositoryMock = new(MockBehavior.Strict);
            RealtimeNotifierMock = new(MockBehavior.Strict);
            TemplateServiceMock = new(MockBehavior.Strict);

            Service = new RecipeManagementService(
                RecipeRepoMock.Object,
                MailServiceMock.Object,
                CacheServiceMock.Object,
                NotificationRepositoryMock.Object,
                RealtimeNotifierMock.Object,
                TemplateServiceMock.Object
            );
        }
    }
}
