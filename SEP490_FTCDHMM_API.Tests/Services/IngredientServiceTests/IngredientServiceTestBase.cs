using AutoMapper;
using Moq;
using SEP490_FTCDHMM_API.Application.Interfaces.ExternalServices;
using SEP490_FTCDHMM_API.Application.Interfaces.Persistence;
using SEP490_FTCDHMM_API.Application.Interfaces.SystemServices;
using SEP490_FTCDHMM_API.Application.Services.Implementations;
using SEP490_FTCDHMM_API.Domain.Entities;

namespace SEP490_FTCDHMM_API.Tests.Services.IngredientServiceTests
{
    public abstract class IngredientServiceTestBase
    {
        protected Mock<IIngredientRepository> IngredientRepositoryMock { get; }
        protected Mock<IIngredientCategoryRepository> IngredientCategoryRepositoryMock { get; }
        protected Mock<IS3ImageService> S3ImageServiceMock { get; }
        protected Mock<IImageRepository> ImageRepositoryMock { get; }
        protected Mock<IIngredientNutritionCalculator> IngredientNutritionCalculatorMock { get; }
        protected Mock<ITranslateService> TranslateServiceMock { get; }
        protected Mock<INutrientRepository> NutrientRepositoryMock { get; }
        protected Mock<IUsdaApiService> UsdaApiServiceMock { get; }
        protected Mock<IMapper> MapperMock { get; }
        protected Mock<IUnitOfWork> UnitOfWorkMock { get; }
        protected Mock<ICacheService> CacheServiceMock { get; }
        protected IngredientService Sut { get; }

        protected IngredientServiceTestBase()
        {
            IngredientRepositoryMock = new Mock<IIngredientRepository>(MockBehavior.Strict);
            IngredientCategoryRepositoryMock = new Mock<IIngredientCategoryRepository>(MockBehavior.Strict);
            S3ImageServiceMock = new Mock<IS3ImageService>(MockBehavior.Strict);
            ImageRepositoryMock = new Mock<IImageRepository>(MockBehavior.Strict);
            IngredientNutritionCalculatorMock = new Mock<IIngredientNutritionCalculator>(MockBehavior.Strict);
            TranslateServiceMock = new Mock<ITranslateService>(MockBehavior.Strict);
            NutrientRepositoryMock = new Mock<INutrientRepository>(MockBehavior.Strict);
            UsdaApiServiceMock = new Mock<IUsdaApiService>(MockBehavior.Strict);
            MapperMock = new Mock<IMapper>(MockBehavior.Strict);
            UnitOfWorkMock = new Mock<IUnitOfWork>(MockBehavior.Strict);
            CacheServiceMock = new Mock<ICacheService>(MockBehavior.Strict);

            Sut = new IngredientService(
                IngredientRepositoryMock.Object,
                IngredientCategoryRepositoryMock.Object,
                S3ImageServiceMock.Object,
                ImageRepositoryMock.Object,
                IngredientNutritionCalculatorMock.Object,
                TranslateServiceMock.Object,
                NutrientRepositoryMock.Object,
                UsdaApiServiceMock.Object,
                MapperMock.Object,
                UnitOfWorkMock.Object,
                CacheServiceMock.Object
            );
        }

        protected Guid NewId() => Guid.NewGuid();

        protected Ingredient CreateIngredient(Guid? id = null)
        {
            return new Ingredient
            {
                Id = id ?? Guid.NewGuid(),
                Name = "Ingredient",
                UpperName = "INGREDIENT",
                NormalizedName = "ingredient",
                Description = "desc",
                LastUpdatedUtc = DateTime.UtcNow,
                Calories = 100,
                ImageId = Guid.NewGuid(),
                Image = new Image { Id = Guid.NewGuid(), Key = "images/default.png" }
            };
        }
    }
}
