using Moq;
using SEP490_FTCDHMM_API.Application.Interfaces.Persistence;
using SEP490_FTCDHMM_API.Application.Interfaces.SystemServices;
using SEP490_FTCDHMM_API.Application.Services.Implementations.RecipeImplementation;
using SEP490_FTCDHMM_API.Application.Services.Interfaces;
using SEP490_FTCDHMM_API.Application.Services.Interfaces.RecipeInterfaces;
using SEP490_FTCDHMM_API.Domain.Entities;
using SEP490_FTCDHMM_API.Domain.ValueObjects;

namespace SEP490_FTCDHMM_API.Tests.Services.RecipeCommandServiceTests
{
    public abstract class RecipeCommandServiceTestBase
    {
        protected Mock<IRecipeRepository> RecipeRepositoryMock { get; }
        protected Mock<ILabelRepository> LabelRepositoryMock { get; }
        protected Mock<IRecipeUserTagRepository> RecipeUserTagRepositoryMock { get; }
        protected Mock<IDraftRecipeRepository> DraftRecipeRepositoryMock { get; }
        protected Mock<IUserSaveRecipeRepository> UserSaveRecipeRepositoryMock { get; }
        protected Mock<IRecipeValidationService> RecipeValidationServiceMock { get; }
        protected Mock<ICacheService> CacheServiceMock { get; }
        protected Mock<IRecipeIngredientRepository> RecipeIngredientRepositoryMock { get; }
        protected Mock<IRecipeImageService> RecipeImageServiceMock { get; }
        protected Mock<IUserFollowRepository> UserFollowRepositoryMock { get; }
        protected Mock<INotificationCommandService> NotificationCommandServiceMock { get; }
        protected Mock<IRecipeNutritionService> RecipeNutritionServiceMock { get; }
        protected Mock<IUnitOfWork> UnitOfWorkMock { get; }

        protected RecipeCommandService Sut { get; }

        protected RecipeCommandServiceTestBase()
        {
            RecipeRepositoryMock = new(MockBehavior.Strict);
            LabelRepositoryMock = new(MockBehavior.Strict);
            RecipeUserTagRepositoryMock = new(MockBehavior.Strict);
            DraftRecipeRepositoryMock = new(MockBehavior.Strict);
            UserSaveRecipeRepositoryMock = new(MockBehavior.Strict);
            RecipeValidationServiceMock = new(MockBehavior.Strict);
            CacheServiceMock = new(MockBehavior.Strict);
            RecipeIngredientRepositoryMock = new(MockBehavior.Strict);
            RecipeImageServiceMock = new(MockBehavior.Strict);
            UserFollowRepositoryMock = new(MockBehavior.Strict);
            NotificationCommandServiceMock = new(MockBehavior.Strict);
            RecipeNutritionServiceMock = new(MockBehavior.Strict);
            UnitOfWorkMock = new(MockBehavior.Strict);

            UnitOfWorkMock
                .Setup(u => u.ExecuteInTransactionAsync(It.IsAny<Func<Task>>()))
                .Returns<Func<Task>>(f => f());

            UnitOfWorkMock
                .Setup(u => u.RegisterAfterCommit(It.IsAny<Func<Task>>()));

            Sut = new RecipeCommandService(
                RecipeRepositoryMock.Object,
                LabelRepositoryMock.Object,
                RecipeUserTagRepositoryMock.Object,
                DraftRecipeRepositoryMock.Object,
                UserSaveRecipeRepositoryMock.Object,
                RecipeValidationServiceMock.Object,
                CacheServiceMock.Object,
                RecipeIngredientRepositoryMock.Object,
                RecipeImageServiceMock.Object,
                UserFollowRepositoryMock.Object,
                NotificationCommandServiceMock.Object,
                RecipeNutritionServiceMock.Object,
                UnitOfWorkMock.Object
            );
        }

        protected Guid NewId() => Guid.NewGuid();

        protected Recipe CreateRecipe(Guid? id = null, Guid? authorId = null)
        {
            return new Recipe
            {
                Id = id ?? Guid.NewGuid(),
                AuthorId = authorId ?? Guid.NewGuid(),
                Status = RecipeStatus.Posted,
                RecipeIngredients = new List<RecipeIngredient>(),
                RecipeUserTags = new List<RecipeUserTag>(),
                Labels = new List<Label>(),
                CookingSteps = new List<CookingStep>()
            };
        }

        protected Label CreateLabel(Guid? id = null)
        {
            return new Label
            {
                Id = id ?? Guid.NewGuid()
            };
        }
    }
}
