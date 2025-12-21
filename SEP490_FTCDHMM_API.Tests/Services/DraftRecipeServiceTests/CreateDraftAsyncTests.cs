using System.Linq.Expressions;
using Moq;
using SEP490_FTCDHMM_API.Application.Dtos.Common;
using SEP490_FTCDHMM_API.Application.Dtos.DraftRecipeDtos;
using SEP490_FTCDHMM_API.Domain.Entities;
using SEP490_FTCDHMM_API.Domain.ValueObjects;
using SEP490_FTCDHMM_API.Shared.Exceptions;

namespace SEP490_FTCDHMM_API.Tests.Services.DraftRecipeServiceTests
{
    public class CreateDraftAsyncTests : DraftRecipeServiceTestBase
    {
        [Fact]
        public async Task CreateDraft_ShouldThrow_WhenStepOrderDuplicate()
        {
            var req = new DraftRecipeRequest
            {
                Name = "A",
                Difficulty = "EASY",
                CookTime = 10,
                CookingSteps =
                {
                    new() { StepOrder = 1 },
                    new() { StepOrder = 1 }
                }
            };

            await Assert.ThrowsAsync<AppException>(() =>
                Sut.CreateDraftAsync(NewId(), req));
        }

        [Fact]
        public async Task CreateDraft_ShouldThrow_WhenImageOrderDuplicate()
        {
            var req = new DraftRecipeRequest
            {
                Name = "A",
                Difficulty = "EASY",
                CookTime = 10,
                CookingSteps =
                {
                    new()
                    {
                        StepOrder = 1,
                        Images =
                        {
                            new() { ImageOrder = 1, Image = new FileUploadModel() },
                            new() { ImageOrder = 1, Image = new FileUploadModel() }
                        }
                    }
                }
            };

            await Assert.ThrowsAsync<AppException>(() =>
                Sut.CreateDraftAsync(NewId(), req));
        }

        [Fact]
        public async Task CreateDraft_ShouldThrow_WhenIngredientDoesNotExist()
        {
            IngredientRepositoryMock
                .Setup(r => r.IdsExistAsync(It.IsAny<List<Guid>>()))
                .ReturnsAsync(false);

            var req = new DraftRecipeRequest
            {
                Name = "A",
                Difficulty = "EASY",
                CookTime = 10,
                Ingredients =
                {
                    new() { IngredientId = NewId(), QuantityGram = 10 }
                }
            };

            await Assert.ThrowsAsync<AppException>(() =>
                Sut.CreateDraftAsync(NewId(), req));
        }

        [Fact]
        public async Task CreateDraft_ShouldThrow_WhenIngredientDuplicate()
        {
            IngredientRepositoryMock
                .Setup(r => r.IdsExistAsync(It.IsAny<List<Guid>>()))
                .ReturnsAsync(true);

            var id = NewId();

            var req = new DraftRecipeRequest
            {
                Name = "A",
                Difficulty = "EASY",
                CookTime = 10,
                Ingredients =
                {
                    new() { IngredientId = id, QuantityGram = 10 },
                    new() { IngredientId = id, QuantityGram = 20 }
                }
            };

            await Assert.ThrowsAsync<AppException>(() =>
                Sut.CreateDraftAsync(NewId(), req));
        }

        [Fact]
        public async Task CreateDraft_ShouldThrow_WhenLabelDoesNotExist()
        {
            IngredientRepositoryMock
                .Setup(r => r.IdsExistAsync(It.IsAny<List<Guid>>()))
                .ReturnsAsync(true);

            LabelRepositoryMock
                .Setup(r => r.IdsExistAsync(It.IsAny<List<Guid>>()))
                .ReturnsAsync(false);

            var req = new DraftRecipeRequest
            {
                Name = "A",
                Difficulty = "EASY",
                CookTime = 10,
                Ingredients = { new() { IngredientId = NewId(), QuantityGram = 1 } },
                LabelIds = { NewId() }
            };

            await Assert.ThrowsAsync<AppException>(() =>
                Sut.CreateDraftAsync(NewId(), req));
        }

        [Fact]
        public async Task CreateDraft_ShouldSucceed()
        {
            IngredientRepositoryMock
                .Setup(r => r.IdsExistAsync(It.IsAny<List<Guid>>()))
                .ReturnsAsync(true);

            LabelRepositoryMock
                .Setup(r => r.IdsExistAsync(It.IsAny<List<Guid>>()))
                .ReturnsAsync(true);

            LabelRepositoryMock
                .Setup(r => r.GetAllAsync(
                    It.IsAny<Expression<Func<Label, bool>>>(),
                    It.IsAny<Func<IQueryable<Label>, IQueryable<Label>>>()))
                .ReturnsAsync(new List<Label>());

            S3ImageServiceMock
                .Setup(s => s.UploadImageAsync(
                    It.IsAny<FileUploadModel>(),
                    StorageFolder.DRAFTS))
                .ReturnsAsync(new Image
                {
                    Id = Guid.NewGuid(),
                    Key = "draft.png",
                    ContentType = "image/png",
                    CreatedAtUTC = DateTime.UtcNow
                });

            DraftRecipeRepositoryMock
                .Setup(r => r.AddAsync(It.IsAny<DraftRecipe>()))
                .ReturnsAsync(new DraftRecipe());

            var req = new DraftRecipeRequest
            {
                Name = "A",
                Difficulty = "EASY",
                CookTime = 10,
                Ingredients = { new() { IngredientId = NewId(), QuantityGram = 1 } },
                CookingSteps = { new() { StepOrder = 1 } },
                LabelIds = new()
            };

            await Sut.CreateDraftAsync(NewId(), req);

            IngredientRepositoryMock.VerifyAll();
            LabelRepositoryMock.VerifyAll();
            DraftRecipeRepositoryMock.VerifyAll();
            S3ImageServiceMock.VerifyAll();
        }
    }
}
