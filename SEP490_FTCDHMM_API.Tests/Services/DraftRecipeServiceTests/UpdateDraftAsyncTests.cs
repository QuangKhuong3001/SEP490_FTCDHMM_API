using System.Linq.Expressions;
using Moq;
using SEP490_FTCDHMM_API.Application.Dtos.Common;
using SEP490_FTCDHMM_API.Application.Dtos.DraftRecipeDtos;
using SEP490_FTCDHMM_API.Application.Dtos.DraftRecipeDtos.DraftCookingStep;
using SEP490_FTCDHMM_API.Application.Dtos.DraftRecipeDtos.DraftCookingStep.DraftCookingStepImage;
using SEP490_FTCDHMM_API.Application.Dtos.DraftRecipeDtos.DraftRecipeIngredient;
using SEP490_FTCDHMM_API.Domain.Entities;
using SEP490_FTCDHMM_API.Domain.ValueObjects;
using SEP490_FTCDHMM_API.Shared.Exceptions;

namespace SEP490_FTCDHMM_API.Tests.Services.DraftRecipeServiceTests
{
    public class UpdateDraftAsyncTests : DraftRecipeServiceTestBase
    {
        private readonly Guid userId = Guid.NewGuid();
        private readonly Guid draftId = Guid.NewGuid();

        private DraftRecipeRequest BuildValidRequest()
        {
            return new DraftRecipeRequest
            {
                Name = "A",
                Description = "B",
                Difficulty = DifficultyValue.Easy.Value,
                CookTime = 10,
                LabelIds = new List<Guid> { Guid.NewGuid() },
                Ingredients = new List<DraftRecipeIngredientRequest>
                {
                    new()
                    {
                        IngredientId = Guid.NewGuid(),
                        QuantityGram = 10
                    }
                },
                TaggedUserIds = new List<Guid>(),
                CookingSteps = new List<DraftCookingStepRequest>
                {
                    new()
                    {
                        StepOrder = 1,
                        Instruction = "X",
                        Images = new List<DraftCookingStepImageRequest>
                        {
                            new()
                            {
                                ImageOrder = 1,
                                Image = new FileUploadModel()
                            }
                        }
                    }
                }
            };
        }

        private void SetupValidIngredientAndLabel()
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
        }

        private void SetupExistingDraft(Guid authorId)
        {
            DraftRecipeRepositoryMock
                .Setup(r => r.GetByIdAsync(
                    draftId,
                    It.IsAny<Func<IQueryable<DraftRecipe>, IQueryable<DraftRecipe>>>()))
                .ReturnsAsync(new DraftRecipe
                {
                    Id = draftId,
                    AuthorId = authorId,
                    ImageId = Guid.NewGuid()
                });

            DraftRecipeRepositoryMock
                .Setup(r => r.DeleteAsync(It.IsAny<DraftRecipe>()))
                .Returns(Task.CompletedTask);

            DraftRecipeRepositoryMock
                .Setup(r => r.AddAsync(It.IsAny<DraftRecipe>()))
                .ReturnsAsync(new DraftRecipe());
        }

        [Fact]
        public async Task ShouldThrow_WhenStepOrderDuplicate()
        {
            var req = BuildValidRequest();
            req.CookingSteps.Add(new DraftCookingStepRequest { StepOrder = 1 });

            await Assert.ThrowsAsync<AppException>(() =>
                Sut.UpdateDraftAsync(userId, draftId, req));
        }

        [Fact]
        public async Task ShouldThrow_WhenImageOrderDuplicate()
        {
            var req = BuildValidRequest();
            req.CookingSteps[0].Images.Add(new DraftCookingStepImageRequest
            {
                ImageOrder = 1,
                Image = new FileUploadModel()
            });

            await Assert.ThrowsAsync<AppException>(() =>
                Sut.UpdateDraftAsync(userId, draftId, req));
        }

        [Fact]
        public async Task ShouldThrow_WhenIngredientNotExist()
        {
            IngredientRepositoryMock
                .Setup(r => r.IdsExistAsync(It.IsAny<List<Guid>>()))
                .ReturnsAsync(false);

            await Assert.ThrowsAsync<AppException>(() =>
                Sut.UpdateDraftAsync(userId, draftId, BuildValidRequest()));
        }

        [Fact]
        public async Task ShouldThrow_WhenDraftNotFound()
        {
            SetupValidIngredientAndLabel();

            DraftRecipeRepositoryMock
                .Setup(r => r.GetByIdAsync(
                    draftId,
                    It.IsAny<Func<IQueryable<DraftRecipe>, IQueryable<DraftRecipe>>>()))
                .ReturnsAsync((DraftRecipe)null!);

            await Assert.ThrowsAsync<AppException>(() =>
                Sut.UpdateDraftAsync(userId, draftId, BuildValidRequest()));
        }

        [Fact]
        public async Task ShouldThrow_WhenNotOwner()
        {
            SetupValidIngredientAndLabel();
            SetupExistingDraft(Guid.NewGuid());

            await Assert.ThrowsAsync<AppException>(() =>
                Sut.UpdateDraftAsync(userId, draftId, BuildValidRequest()));
        }

        [Fact]
        public async Task ShouldUpdate_WhenValid()
        {
            SetupValidIngredientAndLabel();
            SetupExistingDraft(userId);

            S3ImageServiceMock
                .Setup(s => s.UploadImageAsync(
                    It.IsAny<FileUploadModel>(),
                    It.IsAny<StorageFolder>()))
                .ReturnsAsync(new Image
                {
                    Id = Guid.NewGuid(),
                    Key = "x.png",
                    ContentType = "image/png",
                    CreatedAtUTC = DateTime.UtcNow
                });

            UnitOfWorkMock
                .Setup(u => u.RegisterAfterCommit(It.IsAny<Func<Task>>()))
                .Callback<Func<Task>>(f => f());
            ImageRepositoryMock
                .Setup(i => i.MarkDeletedAsync(It.IsAny<Guid>()))
                .Returns(Task.CompletedTask);

            ImageRepositoryMock
                .Setup(i => i.MarkDeletedStepsImageFromDraftAsync(It.IsAny<DraftRecipe>()))
                .Returns(Task.CompletedTask);

            await Sut.UpdateDraftAsync(userId, draftId, BuildValidRequest());

            DraftRecipeRepositoryMock.Verify(
                r => r.DeleteAsync(It.IsAny<DraftRecipe>()),
                Times.Once);

            DraftRecipeRepositoryMock.Verify(
                r => r.AddAsync(It.IsAny<DraftRecipe>()),
                Times.Once);
        }

    }
}
