using AutoMapper;
using Moq;
using SEP490_FTCDHMM_API.Application.Interfaces.Persistence;
using SEP490_FTCDHMM_API.Application.Services.Implementations;
using SEP490_FTCDHMM_API.Application.Services.Implementations.SEP490_FTCDHMM_API.Application.Interfaces;

namespace SEP490_FTCDHMM_API.Tests.Services.IngredientCategoryServiceTests
{
    public abstract class IngredientCategoryServiceTestBase
    {
        protected Mock<IIngredientCategoryRepository> IngredientCateRepositoryMock { get; }
        protected Mock<IMapper> MapperMock { get; }
        protected Mock<ICacheService> CacheServiceMock;

        protected IngredientCategoryService Sut { get; }

        protected IngredientCategoryServiceTestBase()
        {
            IngredientCateRepositoryMock = new Mock<IIngredientCategoryRepository>(MockBehavior.Strict);
            MapperMock = new Mock<IMapper>(MockBehavior.Strict);
            CacheServiceMock = new Mock<ICacheService>(MockBehavior.Strict);

            Sut = new IngredientCategoryService(
                MapperMock.Object,
                CacheServiceMock.Object,
                IngredientCateRepositoryMock.Object
                );
        }

        protected Guid NewId() => Guid.NewGuid();
    }
}
