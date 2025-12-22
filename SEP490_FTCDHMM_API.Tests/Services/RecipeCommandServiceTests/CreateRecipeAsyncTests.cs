using System.Linq.Expressions;
using Moq;
using SEP490_FTCDHMM_API.Application.Dtos.Common;
using SEP490_FTCDHMM_API.Application.Dtos.RecipeDtos;
using SEP490_FTCDHMM_API.Application.Dtos.RecipeDtos.CookingStep;
using SEP490_FTCDHMM_API.Application.Dtos.RecipeDtos.RecipeIngredient;
using SEP490_FTCDHMM_API.Domain.Entities;
using SEP490_FTCDHMM_API.Shared.Exceptions;

namespace SEP490_FTCDHMM_API.Tests.Services.RecipeCommandServiceTests
{
    public class CreateRecipeAsyncTests : RecipeCommandServiceTestBase
    {
        [Fact]
        public async Task CreateRecipeAsync_ShouldThrow_WhenImageAndExistingImageProvided()
        {
            var request = new CreateRecipeRequest
            {
                Name = "Test",
                Difficulty = "Easy",
                CookTime = 10,
                Image = new FileUploadModel(),
                ExistingMainImageUrl = "url"
            };

            await Assert.ThrowsAsync<AppException>(() =>
                Sut.CreateRecipeAsync(Guid.NewGuid(), request));
        }

        [Fact]
        public async Task CreateRecipeAsync_ShouldThrow_WhenValidateLabelsFail()
        {
            var request = new CreateRecipeRequest
            {
                Name = "Test",
                Difficulty = "Easy",
                CookTime = 10,
                LabelIds = new List<Guid> { Guid.NewGuid() }
            };

            RecipeValidationServiceMock
                .Setup(x => x.ValidateLabelsAsync(request.LabelIds))
                .ThrowsAsync(new AppException(AppResponseCode.NOT_FOUND));

            await Assert.ThrowsAsync<AppException>(() =>
                Sut.CreateRecipeAsync(Guid.NewGuid(), request));
        }

        [Fact]
        public async Task CreateRecipeAsync_ShouldThrow_WhenValidateIngredientsFail()
        {
            var request = new CreateRecipeRequest
            {
                Name = "Test",
                Difficulty = "Easy",
                CookTime = 10,
                Ingredients = new List<RecipeIngredientRequest>
        {
            new RecipeIngredientRequest
            {
                IngredientId = Guid.NewGuid(),
                QuantityGram = 100
            }
        }
            };

            RecipeValidationServiceMock
                .Setup(x => x.ValidateLabelsAsync(It.IsAny<IEnumerable<Guid>>()))
                .Returns(Task.CompletedTask);

            RecipeValidationServiceMock
                .Setup(x => x.ValidateIngredientsAsync(It.IsAny<IEnumerable<Guid>>()))
                .ThrowsAsync(new AppException(AppResponseCode.NOT_FOUND));

            await Assert.ThrowsAsync<AppException>(() =>
                Sut.CreateRecipeAsync(Guid.NewGuid(), request));
        }

        [Fact]
        public async Task CreateRecipeAsync_ShouldThrow_WhenValidateCookingStepsFail()
        {
            var request = new CreateRecipeRequest
            {
                Name = "Test",
                Difficulty = "Easy",
                CookTime = 10,
                CookingSteps = new List<CookingStepRequest>
        {
            new CookingStepRequest
            {
                StepOrder = 1,
                Instruction = "A"
            }
        }
            };

            RecipeValidationServiceMock
                .Setup(x => x.ValidateLabelsAsync(It.IsAny<IEnumerable<Guid>>()))
                .Returns(Task.CompletedTask);

            RecipeValidationServiceMock
                .Setup(x => x.ValidateIngredientsAsync(It.IsAny<IEnumerable<Guid>>()))
                .Returns(Task.CompletedTask);

            RecipeValidationServiceMock
                .Setup(x => x.ValidateCookingStepsAsync(request.CookingSteps))
                .ThrowsAsync(new AppException(AppResponseCode.INVALID_ACTION));

            await Assert.ThrowsAsync<AppException>(() =>
                Sut.CreateRecipeAsync(Guid.NewGuid(), request));
        }

        [Fact]
        public async Task CreateRecipeAsync_ShouldThrow_WhenValidateTaggedUsersFail()
        {
            var userId = Guid.NewGuid();

            var request = new CreateRecipeRequest
            {
                Name = "Test",
                Difficulty = "Easy",
                CookTime = 10,
                TaggedUserIds = new List<Guid> { Guid.NewGuid() }
            };

            RecipeValidationServiceMock
                .Setup(x => x.ValidateLabelsAsync(It.IsAny<IEnumerable<Guid>>()))
                .Returns(Task.CompletedTask);

            RecipeValidationServiceMock
                .Setup(x => x.ValidateIngredientsAsync(It.IsAny<IEnumerable<Guid>>()))
                .Returns(Task.CompletedTask);

            RecipeValidationServiceMock
                .Setup(x => x.ValidateCookingStepsAsync(It.IsAny<IEnumerable<CookingStepRequest>>()))
                .Returns(Task.CompletedTask);

            RecipeValidationServiceMock
                .Setup(x => x.ValidateTaggedUsersAsync(userId, request.TaggedUserIds))
                .ThrowsAsync(new AppException(AppResponseCode.NOT_FOUND));

            await Assert.ThrowsAsync<AppException>(() =>
                Sut.CreateRecipeAsync(userId, request));
        }

        [Fact]
        public async Task CreateRecipeAsync_ShouldDeleteDraft_WhenDraftExists()
        {
            var userId = Guid.NewGuid();
            var draft = new DraftRecipe { Id = Guid.NewGuid() };

            RecipeValidationServiceMock
                .Setup(x => x.ValidateLabelsAsync(It.IsAny<IEnumerable<Guid>>()))
                .Returns(Task.CompletedTask);

            RecipeValidationServiceMock
                .Setup(x => x.ValidateIngredientsAsync(It.IsAny<IEnumerable<Guid>>()))
                .Returns(Task.CompletedTask);

            RecipeValidationServiceMock
                .Setup(x => x.ValidateCookingStepsAsync(It.IsAny<IEnumerable<CookingStepRequest>>()))
                .Returns(Task.CompletedTask);

            RecipeValidationServiceMock
                .Setup(x => x.ValidateTaggedUsersAsync(userId, It.IsAny<IEnumerable<Guid>>()))
                .Returns(Task.CompletedTask);

            DraftRecipeRepositoryMock
                .Setup(x => x.GetByIdAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<Func<IQueryable<DraftRecipe>, IQueryable<DraftRecipe>>>()
                ))
                .ReturnsAsync(draft);

            DraftRecipeRepositoryMock
                .Setup(x => x.DeleteAsync(draft))
                .Returns(Task.CompletedTask);

            LabelRepositoryMock
                .Setup(x => x.GetAllAsync(It.IsAny<Expression<Func<Label, bool>>>(), null))
                .ReturnsAsync(new List<Label>());

            RecipeRepositoryMock
                .Setup(x => x.AddAsync(It.IsAny<Recipe>()))
                .ReturnsAsync((Recipe r) => r);

            RecipeRepositoryMock
                .Setup(x => x.GetByIdAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<Func<IQueryable<Recipe>, IQueryable<Recipe>>>()
                ))
                .ReturnsAsync(CreateRecipe());

            RecipeNutritionServiceMock
                .Setup(x => x.AggregateRecipeAsync(It.IsAny<Recipe>()))
                .Returns(Task.CompletedTask);

            CacheServiceMock
                .Setup(x => x.RemoveByPrefixAsync(It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            RecipeImageServiceMock
                .Setup(x => x.SetRecipeImageAsync(
                    It.IsAny<Recipe>(),
                    null,
                    null
                ))
                .Returns(Task.CompletedTask);

            RecipeImageServiceMock
                .Setup(x => x.CreateCookingStepsAsync(
                    It.IsAny<IEnumerable<CookingStepRequest>>(),
                    It.IsAny<Recipe>()))
                .ReturnsAsync(new List<CookingStep>());

            await Sut.CreateRecipeAsync(userId, new CreateRecipeRequest
            {
                Name = "Test",
                Difficulty = "Easy",
                CookTime = 10
            });

            DraftRecipeRepositoryMock.Verify(x => x.DeleteAsync(draft), Times.Once);
        }


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
            new RecipeIngredientRequest
            {
                IngredientId = Guid.NewGuid(),
                QuantityGram = 100
            }
        },
                CookingSteps = new List<CookingStepRequest>
        {
            new CookingStepRequest
            {
                StepOrder = 1,
                Instruction = "A"
            }
        }
            };

            RecipeValidationServiceMock.Setup(x => x.ValidateLabelsAsync(request.LabelIds))
                .Returns(Task.CompletedTask);

            RecipeValidationServiceMock.Setup(x => x.ValidateIngredientsAsync(It.IsAny<IEnumerable<Guid>>()))
                .Returns(Task.CompletedTask);

            RecipeValidationServiceMock.Setup(x => x.ValidateCookingStepsAsync(request.CookingSteps))
                .Returns(Task.CompletedTask);

            RecipeValidationServiceMock.Setup(x => x.ValidateTaggedUsersAsync(userId, request.TaggedUserIds))
                .Returns(Task.CompletedTask);

            DraftRecipeRepositoryMock
                .Setup(x => x.GetByIdAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<Func<IQueryable<DraftRecipe>, IQueryable<DraftRecipe>>>()
                ))
                .ReturnsAsync((DraftRecipe)null!);

            LabelRepositoryMock
                .Setup(x => x.GetAllAsync(It.IsAny<Expression<Func<Label, bool>>>(), null))
                .ReturnsAsync(new List<Label> { CreateLabel(labelId) });

            RecipeImageServiceMock
                .Setup(x => x.SetRecipeImageAsync(It.IsAny<Recipe>(), null, null))
                .Returns(Task.CompletedTask);

            RecipeImageServiceMock
                .Setup(x => x.CreateCookingStepsAsync(request.CookingSteps, It.IsAny<Recipe>()))
                .ReturnsAsync(new List<CookingStep>());

            RecipeRepositoryMock
                .Setup(x => x.AddAsync(It.IsAny<Recipe>()))
                .ReturnsAsync((Recipe r) => r);

            RecipeRepositoryMock
                .Setup(x => x.GetByIdAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<Func<IQueryable<Recipe>, IQueryable<Recipe>>>()
                ))
                .ReturnsAsync(CreateRecipe(Guid.NewGuid()));

            RecipeNutritionServiceMock
                .Setup(x => x.AggregateRecipeAsync(It.IsAny<Recipe>()))
                .Returns(Task.CompletedTask);

            CacheServiceMock
                .Setup(x => x.RemoveByPrefixAsync(It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            await Sut.CreateRecipeAsync(userId, request);

            RecipeRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Recipe>()), Times.Once);
        }
    }
}
