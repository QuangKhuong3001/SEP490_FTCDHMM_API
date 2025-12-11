using System.Linq.Expressions;
using Moq;
using SEP490_FTCDHMM_API.Application.Dtos.RecipeDtos;
using SEP490_FTCDHMM_API.Application.Dtos.RecipeDtos.CookingStep;
using SEP490_FTCDHMM_API.Application.Dtos.RecipeDtos.RecipeIngredient;
using SEP490_FTCDHMM_API.Domain.Entities;
using SEP490_FTCDHMM_API.Shared.Exceptions;

namespace SEP490_FTCDHMM_API.Tests.Services.RecipeCommandServiceTests
{
    public class CopyRecipeAsyncTests : RecipeCommandServiceTestBase
    {
        [Fact]
        public async Task CopyRecipeAsync_ShouldThrow_WhenParentNotFound()
        {
            var userId = Guid.NewGuid();
            var parentId = Guid.NewGuid();

            var request = new CopyRecipeRequest
            {
                Name = "A",
                Difficulty = "Easy",
                CookTime = 10
            };

            RecipeValidationServiceMock.Setup(x => x.ValidateLabelsAsync(request.LabelIds)).Returns(Task.CompletedTask);
            RecipeValidationServiceMock.Setup(x => x.ValidateIngredientsAsync(It.IsAny<IEnumerable<Guid>>())).Returns(Task.CompletedTask);
            RecipeValidationServiceMock.Setup(x => x.ValidateCookingStepsAsync(request.CookingSteps)).Returns(Task.CompletedTask);
            RecipeValidationServiceMock.Setup(x => x.ValidateTaggedUsersAsync(userId, request.TaggedUserIds)).Returns(Task.CompletedTask);

            RecipeRepositoryMock.Setup(x => x.GetByIdAsync(parentId, null))
                .ReturnsAsync((Recipe)null!);

            await Assert.ThrowsAsync<AppException>(() =>
                Sut.CopyRecipeAsync(userId, parentId, request));
        }

        [Fact]
        public async Task CopyRecipeAsync_ShouldCopySuccessfully()
        {
            var userId = Guid.NewGuid();
            var parentId = Guid.NewGuid();
            var labelId = Guid.NewGuid();

            var parentRecipe = CreateRecipe(parentId);

            var request = new CopyRecipeRequest
            {
                Name = "Copy",
                Difficulty = "Medium",
                CookTime = 30,
                LabelIds = new List<Guid> { labelId },
                Ingredients = new List<RecipeIngredientRequest>
                {
                    new RecipeIngredientRequest { IngredientId = Guid.NewGuid(), QuantityGram = 40 }
                },
                CookingSteps = new List<CookingStepRequest>
                {
                    new CookingStepRequest { StepOrder = 1, Instruction = "Mix" }
                }
            };

            RecipeValidationServiceMock.Setup(x => x.ValidateLabelsAsync(request.LabelIds)).Returns(Task.CompletedTask);
            RecipeValidationServiceMock.Setup(x => x.ValidateIngredientsAsync(It.IsAny<IEnumerable<Guid>>())).Returns(Task.CompletedTask);
            RecipeValidationServiceMock.Setup(x => x.ValidateCookingStepsAsync(request.CookingSteps)).Returns(Task.CompletedTask);
            RecipeValidationServiceMock.Setup(x => x.ValidateTaggedUsersAsync(userId, request.TaggedUserIds)).Returns(Task.CompletedTask);

            RecipeRepositoryMock.Setup(x => x.GetByIdAsync(parentId, null))
                .ReturnsAsync(parentRecipe);

            DraftRecipeRepositoryMock.Setup(x => x.GetDraftByAuthorIdAsync(userId))
                .ReturnsAsync((DraftRecipe)null!);

            var labels = new List<Label> { CreateLabel(labelId) };

            LabelRepositoryMock.Setup(x =>
                x.GetAllAsync(It.IsAny<Expression<Func<Label, bool>>>(), null))
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

            var generated = CreateRecipe(Guid.NewGuid());

            RecipeRepositoryMock.Setup(x =>
                x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<Func<IQueryable<Recipe>, IQueryable<Recipe>>>()))
                .ReturnsAsync(generated);

            RecipeNutritionServiceMock.Setup(x =>
                x.AggregateRecipeAsync(It.IsAny<Recipe>()))
                .Returns(Task.CompletedTask);

            await Sut.CopyRecipeAsync(userId, parentId, request);

            RecipeRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Recipe>()), Times.Once);
        }
    }
}
