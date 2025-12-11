using System.Linq.Expressions;
using Moq;
using SEP490_FTCDHMM_API.Application.Dtos.RecipeDtos;
using SEP490_FTCDHMM_API.Application.Dtos.RecipeDtos.CookingStep;
using SEP490_FTCDHMM_API.Application.Dtos.RecipeDtos.RecipeIngredient;
using SEP490_FTCDHMM_API.Domain.Entities;

namespace SEP490_FTCDHMM_API.Tests.Services.RecipeCommandServiceTests
{
    public class CreateRecipeAsyncTests : RecipeCommandServiceTestBase
    {
        [Fact]
        public async Task CreateRecipeAsync_ShouldCreateSuccessfully()
        {
            var userId = Guid.NewGuid();
            var labelId = Guid.NewGuid();

            var request = new CreateRecipeRequest
            {
                Name = "Test",
                Difficulty = "Easy",
                CookTime = 10,
                LabelIds = new List<Guid> { labelId },
                Ingredients = new List<RecipeIngredientRequest>
                {
                    new RecipeIngredientRequest { IngredientId = Guid.NewGuid(), QuantityGram = 100 }
                },
                CookingSteps = new List<CookingStepRequest>
                {
                    new CookingStepRequest { StepOrder = 1, Instruction = "A" }
                }
            };

            RecipeValidationServiceMock.Setup(x => x.ValidateLabelsAsync(request.LabelIds)).Returns(Task.CompletedTask);
            RecipeValidationServiceMock.Setup(x => x.ValidateIngredientsAsync(It.IsAny<IEnumerable<Guid>>())).Returns(Task.CompletedTask);
            RecipeValidationServiceMock.Setup(x => x.ValidateCookingStepsAsync(request.CookingSteps)).Returns(Task.CompletedTask);
            RecipeValidationServiceMock.Setup(x => x.ValidateTaggedUsersAsync(userId, request.TaggedUserIds)).Returns(Task.CompletedTask);

            DraftRecipeRepositoryMock.Setup(x => x.GetDraftByAuthorIdAsync(userId))
                .ReturnsAsync((DraftRecipe)null!);

            var labels = new List<Label> { CreateLabel(labelId) };

            LabelRepositoryMock
                .Setup(x => x.GetAllAsync(It.IsAny<Expression<Func<Label, bool>>>(), null))
                .ReturnsAsync(labels);

            RecipeImageServiceMock
                .Setup(x => x.SetRecipeImageAsync(It.IsAny<Recipe>(), null, userId))
                .Returns(Task.CompletedTask);

            RecipeImageServiceMock
                .Setup(x => x.CreateCookingStepsAsync(request.CookingSteps, It.IsAny<Recipe>(), userId))
                .ReturnsAsync(new List<CookingStep>());

            RecipeRepositoryMock.Setup(x =>
                x.AddAsync(It.IsAny<Recipe>()))
                .ReturnsAsync((Recipe r) => r);

            RecipeRepositoryMock.Setup(x =>
                    x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<Func<IQueryable<Recipe>, IQueryable<Recipe>>>()))
                .ReturnsAsync(CreateRecipe(Guid.NewGuid()));

            RecipeNutritionServiceMock
                .Setup(x => x.AggregateRecipeAsync(It.IsAny<Recipe>()))
                .Returns(Task.CompletedTask);

            await Sut.CreateRecipeAsync(userId, request);

            RecipeRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Recipe>()), Times.Once);
        }
    }
}
