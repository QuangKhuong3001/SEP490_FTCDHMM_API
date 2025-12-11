using Moq;
using SEP490_FTCDHMM_API.Application.Interfaces.Persistence;
using SEP490_FTCDHMM_API.Application.Services.Implementations.RecipeImplementation;
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
        protected Mock<IRecipeImageService> RecipeImageServiceMock { get; }
        protected Mock<IRecipeNutritionService> RecipeNutritionServiceMock { get; }
        protected Mock<IRecipeIngredientRepository> RecipeIngredientRepositoryMock { get; }

        protected RecipeCommandService Sut { get; }

        protected RecipeCommandServiceTestBase()
        {
            RecipeRepositoryMock = new Mock<IRecipeRepository>(MockBehavior.Strict);
            LabelRepositoryMock = new Mock<ILabelRepository>(MockBehavior.Strict);
            RecipeUserTagRepositoryMock = new Mock<IRecipeUserTagRepository>(MockBehavior.Strict);
            DraftRecipeRepositoryMock = new Mock<IDraftRecipeRepository>(MockBehavior.Strict);
            UserSaveRecipeRepositoryMock = new Mock<IUserSaveRecipeRepository>(MockBehavior.Strict);
            RecipeValidationServiceMock = new Mock<IRecipeValidationService>(MockBehavior.Strict);
            RecipeImageServiceMock = new Mock<IRecipeImageService>(MockBehavior.Strict);
            RecipeNutritionServiceMock = new Mock<IRecipeNutritionService>(MockBehavior.Strict);
            RecipeIngredientRepositoryMock = new Mock<IRecipeIngredientRepository>(MockBehavior.Strict);

            Sut = new RecipeCommandService(
                RecipeRepositoryMock.Object,
                LabelRepositoryMock.Object,
                RecipeUserTagRepositoryMock.Object,
                DraftRecipeRepositoryMock.Object,
                UserSaveRecipeRepositoryMock.Object,
                RecipeValidationServiceMock.Object,
                RecipeIngredientRepositoryMock.Object,
                RecipeImageServiceMock.Object,
                RecipeNutritionServiceMock.Object
            );
        }

        protected Recipe CreateRecipe(Guid id)
        {
            RecipeStatus status = RecipeStatus.Posted;
            return new Recipe
            {
                Id = id,
                Status = status,
                RecipeIngredients = new List<RecipeIngredient>(),
                RecipeUserTags = new List<RecipeUserTag>(),
                Labels = new List<Label>(),
                CookingSteps = new List<CookingStep>()
            };
        }

        protected Label CreateLabel(Guid id)
        {
            return new Label { Id = id };
        }
    }
}
