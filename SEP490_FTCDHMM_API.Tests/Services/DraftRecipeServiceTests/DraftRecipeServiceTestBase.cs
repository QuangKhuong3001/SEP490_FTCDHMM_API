using AutoMapper;
using Moq;
using SEP490_FTCDHMM_API.Application.Interfaces.ExternalServices;
using SEP490_FTCDHMM_API.Application.Interfaces.Persistence;
using SEP490_FTCDHMM_API.Application.Services.Implementations;

namespace SEP490_FTCDHMM_API.Tests.Services.DraftRecipeServiceTests
{
    public abstract class DraftRecipeServiceTestBase
    {
        protected Mock<IDraftRecipeRepository> DraftRecipeRepositoryMock { get; }
        protected Mock<IUserRepository> UserRepositoryMock { get; }
        protected Mock<ILabelRepository> LabelRepositoryMock { get; }
        protected Mock<IImageRepository> ImageRepositoryMock { get; }
        protected Mock<IIngredientRepository> IngredientRepositoryMock { get; }
        protected Mock<IS3ImageService> S3Mock { get; }
        protected Mock<IMapper> MapperMock { get; }

        protected DraftRecipeService Sut { get; }

        protected DraftRecipeServiceTestBase()
        {
            DraftRecipeRepositoryMock = new Mock<IDraftRecipeRepository>(MockBehavior.Strict);
            UserRepositoryMock = new Mock<IUserRepository>(MockBehavior.Strict);
            LabelRepositoryMock = new Mock<ILabelRepository>(MockBehavior.Strict);
            ImageRepositoryMock = new Mock<IImageRepository>(MockBehavior.Strict);
            IngredientRepositoryMock = new Mock<IIngredientRepository>(MockBehavior.Strict);
            S3Mock = new Mock<IS3ImageService>(MockBehavior.Strict);
            MapperMock = new Mock<IMapper>(MockBehavior.Strict);

            Sut = new DraftRecipeService(
                MapperMock.Object,
                DraftRecipeRepositoryMock.Object,
                UserRepositoryMock.Object,
                LabelRepositoryMock.Object,
                S3Mock.Object,
                ImageRepositoryMock.Object,
                IngredientRepositoryMock.Object
            );
        }

        protected Guid NewId() => Guid.NewGuid();
    }
}
