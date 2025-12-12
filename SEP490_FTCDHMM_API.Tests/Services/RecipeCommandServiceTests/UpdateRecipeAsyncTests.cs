using System.Linq.Expressions;
using Moq;
using SEP490_FTCDHMM_API.Application.Dtos.RecipeDtos;
using SEP490_FTCDHMM_API.Application.Dtos.RecipeDtos.CookingStep;
using SEP490_FTCDHMM_API.Application.Dtos.RecipeDtos.RecipeIngredient;
using SEP490_FTCDHMM_API.Domain.Entities;
using SEP490_FTCDHMM_API.Shared.Exceptions;

namespace SEP490_FTCDHMM_API.Tests.Services.RecipeCommandServiceTests
{
    public class UpdateRecipeAsyncTests : RecipeCommandServiceTestBase
    {
        [Fact]
        public async Task UpdateRecipeAsync_ShouldThrow_WhenRecipeNotFound()
        {
            var userId = Guid.NewGuid();
            var recipeId = Guid.NewGuid();

            var request = new UpdateRecipeRequest
            {
                Name = "Test",
                Difficulty = "Easy",
                CookTime = 10
            };

            RecipeValidationServiceMock.Setup(x => x.ValidateLabelsAsync(request.LabelIds)).Returns(Task.CompletedTask);
            RecipeValidationServiceMock.Setup(x => x.ValidateIngredientsAsync(It.IsAny<IEnumerable<Guid>>())).Returns(Task.CompletedTask);
            RecipeValidationServiceMock.Setup(x => x.ValidateCookingStepsAsync(request.CookingSteps)).Returns(Task.CompletedTask);
            RecipeValidationServiceMock.Setup(x => x.ValidateTaggedUsersAsync(userId, request.TaggedUserIds)).Returns(Task.CompletedTask);

            RecipeRepositoryMock.Setup(x =>
                    x.GetByIdAsync(recipeId, It.IsAny<Func<IQueryable<Recipe>, IQueryable<Recipe>>>()))
                .ReturnsAsync((Recipe)null!);

            await Assert.ThrowsAsync<AppException>(() =>
                Sut.UpdateRecipeAsync(userId, recipeId, request));
        }

        [Fact]
        public async Task UpdateRecipeAsync_ShouldThrow_WhenNotOwner()
        {
            var userId = Guid.NewGuid();
            var recipeId = Guid.NewGuid();
            var recipe = CreateRecipe(recipeId);

            var request = new UpdateRecipeRequest
            {
                Name = "Test",
                Difficulty = "Easy",
                CookTime = 10
            };

            RecipeValidationServiceMock.Setup(x => x.ValidateLabelsAsync(request.LabelIds)).Returns(Task.CompletedTask);
            RecipeValidationServiceMock.Setup(x => x.ValidateIngredientsAsync(It.IsAny<IEnumerable<Guid>>())).Returns(Task.CompletedTask);
            RecipeValidationServiceMock.Setup(x => x.ValidateCookingStepsAsync(request.CookingSteps)).Returns(Task.CompletedTask);
            RecipeValidationServiceMock.Setup(x => x.ValidateTaggedUsersAsync(userId, request.TaggedUserIds)).Returns(Task.CompletedTask);

            RecipeRepositoryMock.Setup(x =>
                    x.GetByIdAsync(recipeId, It.IsAny<Func<IQueryable<Recipe>, IQueryable<Recipe>>>()))
                .ReturnsAsync(recipe);

            RecipeValidationServiceMock.Setup(x => x.ValidateRecipeOwnerAsync(userId, recipe))
                .ThrowsAsync(new AppException(AppResponseCode.FORBIDDEN));

            await Assert.ThrowsAsync<AppException>(() =>
                Sut.UpdateRecipeAsync(userId, recipeId, request));
        }

        [Fact]
        public async Task UpdateRecipeAsync_ShouldUpdateSuccessfully()
        {
            var userId = Guid.NewGuid();
            var recipeId = Guid.NewGuid();
            var labelId = Guid.NewGuid();

            var recipe = CreateRecipe(recipeId);

            var request = new UpdateRecipeRequest
            {
                Name = "Updated",
                Difficulty = "Hard",
                CookTime = 50,
                LabelIds = new List<Guid> { labelId },
                Ingredients = new List<RecipeIngredientRequest>
                {
                    new RecipeIngredientRequest { IngredientId = Guid.NewGuid(), QuantityGram = 200 }
                },
                CookingSteps = new List<CookingStepRequest>
                {
                    new CookingStepRequest { StepOrder = 1, Instruction = "Do A" }
                }
            };

            RecipeValidationServiceMock.Setup(x => x.ValidateLabelsAsync(request.LabelIds)).Returns(Task.CompletedTask);
            RecipeValidationServiceMock.Setup(x => x.ValidateIngredientsAsync(It.IsAny<IEnumerable<Guid>>())).Returns(Task.CompletedTask);
            RecipeValidationServiceMock.Setup(x => x.ValidateCookingStepsAsync(request.CookingSteps)).Returns(Task.CompletedTask);
            RecipeValidationServiceMock.Setup(x => x.ValidateTaggedUsersAsync(userId, request.TaggedUserIds)).Returns(Task.CompletedTask);
            RecipeValidationServiceMock.Setup(x => x.ValidateRecipeOwnerAsync(userId, recipe)).Returns(Task.CompletedTask);

            var labels = new List<Label> { new Label { Id = labelId } };

            RecipeRepositoryMock.Setup(x =>
                    x.GetByIdAsync(recipeId, It.IsAny<Func<IQueryable<Recipe>, IQueryable<Recipe>>>()))
                .ReturnsAsync(recipe);

            LabelRepositoryMock.Setup(x =>
                x.GetAllAsync(It.IsAny<Expression<Func<Label, bool>>>(), null))
                .ReturnsAsync(labels);

            RecipeIngredientRepositoryMock.Setup(x =>
                x.DeleteRangeAsync(It.IsAny<IEnumerable<RecipeIngredient>>()))
                .Returns(Task.CompletedTask);

            RecipeImageServiceMock.Setup(x =>
                x.ReplaceRecipeImageAsync(recipe, null))
                .Returns(Task.CompletedTask);

            RecipeUserTagRepositoryMock.Setup(x =>
                x.GetAllAsync(It.IsAny<Expression<Func<RecipeUserTag, bool>>>(), null))
                .ReturnsAsync(new List<RecipeUserTag>());

            RecipeImageServiceMock.Setup(x =>
                x.ReplaceCookingStepsAsync(recipe.Id, request.CookingSteps))
                .Returns(Task.CompletedTask);

            RecipeRepositoryMock.Setup(x => x.UpdateAsync(recipe))
                .Returns(Task.CompletedTask);

            RecipeRepositoryMock.Setup(x =>
                    x.GetByIdAsync(recipeId, It.IsAny<Func<IQueryable<Recipe>, IQueryable<Recipe>>>()))
                .ReturnsAsync(recipe);

            RecipeNutritionServiceMock.Setup(x =>
                x.AggregateRecipeAsync(recipe))
                .Returns(Task.CompletedTask);

            await Sut.UpdateRecipeAsync(userId, recipeId, request);

            RecipeRepositoryMock.Verify(x => x.UpdateAsync(recipe), Times.Once);
        }
    }
}
