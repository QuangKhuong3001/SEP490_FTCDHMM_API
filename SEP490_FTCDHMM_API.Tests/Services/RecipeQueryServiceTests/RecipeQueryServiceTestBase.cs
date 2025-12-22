using AutoMapper;
using Moq;
using SEP490_FTCDHMM_API.Application.Dtos.RecipeDtos.Rating;
using SEP490_FTCDHMM_API.Application.Dtos.RecipeDtos.Response;
using SEP490_FTCDHMM_API.Application.Interfaces.Persistence;
using SEP490_FTCDHMM_API.Application.Interfaces.SystemServices;
using SEP490_FTCDHMM_API.Application.Services.Implementations.RecipeImplementation;

namespace SEP490_FTCDHMM_API.Tests.Services.RecipeQueryServiceTests
{
    public abstract class RecipeQueryServiceTestBase
    {
        protected Mock<IRecipeRepository> RecipeRepositoryMock { get; }
        protected Mock<IUserRepository> UserRepositoryMock { get; }
        protected Mock<IUserSaveRecipeRepository> UserSaveRecipeRepositoryMock { get; }
        protected Mock<IUserRecipeViewRepository> UserRecipeViewRepositoryMock { get; }
        protected Mock<IMapper> MapperMock { get; }
        protected Mock<ICacheService> CacheServiceMock { get; }

        protected RecipeQueryService Sut { get; }

        protected RecipeQueryServiceTestBase()
        {
            RecipeRepositoryMock = new Mock<IRecipeRepository>(MockBehavior.Strict);
            UserRepositoryMock = new Mock<IUserRepository>(MockBehavior.Strict);
            UserSaveRecipeRepositoryMock = new Mock<IUserSaveRecipeRepository>(MockBehavior.Strict);
            UserRecipeViewRepositoryMock = new Mock<IUserRecipeViewRepository>(MockBehavior.Strict);
            MapperMock = new Mock<IMapper>(MockBehavior.Strict);
            CacheServiceMock = new Mock<ICacheService>(MockBehavior.Strict);

            CacheServiceMock
                .Setup(c => c.GetAsync<RecipeDetailsResponse>(It.IsAny<string>()))
                .ReturnsAsync((RecipeDetailsResponse?)null);

            CacheServiceMock
                .Setup(c => c.GetAsync<RecipeRatingResponse>(It.IsAny<string>()))
                .ReturnsAsync((RecipeRatingResponse?)null);

            CacheServiceMock
                .Setup(c => c.SetAsync(
                    It.IsAny<string>(),
                    It.IsAny<RecipeRatingResponse>(),
                    It.IsAny<TimeSpan>()))
                .Returns(Task.CompletedTask);

            CacheServiceMock
                .Setup(c => c.SetAsync(
                    It.IsAny<string>(),
                    It.IsAny<RecipeDetailsResponse>(),
                    It.IsAny<TimeSpan>()))
                .Returns(Task.CompletedTask);

            Sut = new RecipeQueryService(
                RecipeRepositoryMock.Object,
                UserRepositoryMock.Object,
                UserSaveRecipeRepositoryMock.Object,
                UserRecipeViewRepositoryMock.Object,
                CacheServiceMock.Object,
                MapperMock.Object
            );
        }

        protected Guid NewId() => Guid.NewGuid();
    }
}
