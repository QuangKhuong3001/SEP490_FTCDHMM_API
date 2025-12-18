using AutoMapper;
using Moq;
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
        protected Mock<IIngredientRepository> IngredientRepositoryMock { get; }
        protected Mock<ILabelRepository> LabelRepositoryMock { get; }
        protected Mock<IUserRecipeViewRepository> UserRecipeViewRepositoryMock { get; }
        protected Mock<IMapper> MapperMock { get; }
        protected Mock<ICacheService> CacheServiceMock;

        protected RecipeQueryService Sut { get; }

        protected RecipeQueryServiceTestBase()
        {
            RecipeRepositoryMock = new Mock<IRecipeRepository>(MockBehavior.Strict);
            UserRepositoryMock = new Mock<IUserRepository>(MockBehavior.Strict);
            UserSaveRecipeRepositoryMock = new Mock<IUserSaveRecipeRepository>(MockBehavior.Strict);
            IngredientRepositoryMock = new Mock<IIngredientRepository>(MockBehavior.Strict);
            LabelRepositoryMock = new Mock<ILabelRepository>(MockBehavior.Strict);
            UserRecipeViewRepositoryMock = new Mock<IUserRecipeViewRepository>(MockBehavior.Strict);
            MapperMock = new Mock<IMapper>(MockBehavior.Strict);
            CacheServiceMock = new Mock<ICacheService>(MockBehavior.Strict);

            Sut = new RecipeQueryService(
                RecipeRepositoryMock.Object,
                UserRepositoryMock.Object,
                UserSaveRecipeRepositoryMock.Object,
                IngredientRepositoryMock.Object,
                LabelRepositoryMock.Object,
                UserRecipeViewRepositoryMock.Object,
                CacheServiceMock.Object,
                MapperMock.Object

            );
        }

        protected Guid NewId() => Guid.NewGuid();
    }
}
