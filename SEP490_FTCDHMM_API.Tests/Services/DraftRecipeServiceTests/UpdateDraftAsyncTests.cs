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
                    new DraftRecipeIngredientRequest
                    {
                        IngredientId = Guid.NewGuid(),
                        QuantityGram = 10
                    }
                },
                TaggedUserIds = new List<Guid>(),
                CookingSteps = new List<DraftCookingStepRequest>
                {
                    new DraftCookingStepRequest
                    {
                        StepOrder = 1,
                        Instruction = "X",
                        Images = new List<DraftCookingStepImageRequest>
                        {
                            new DraftCookingStepImageRequest
                            {
                                ImageOrder = 1,
                                Image = new FileUploadModel
                                {
                                    FileName = "a.png",
                                    Content = new MemoryStream(new byte[1]),
                                    ContentType = "image/png"
                                }
                            }
                        }
                    }
                }
            };
        }

        private void SetupValidIngredientLabel(DraftRecipeRequest req)
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
                    null
                ))
                .ReturnsAsync(new List<Label>());
        }

        private void SetupDraft()
        {
            DraftRecipeRepositoryMock
                .Setup(r => r.GetByIdAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<Func<IQueryable<DraftRecipe>, IQueryable<DraftRecipe>>>()))
                .ReturnsAsync(new DraftRecipe
                {
                    Id = draftId,
                    AuthorId = userId,
                    ImageId = Guid.NewGuid(),
                    DraftCookingSteps = new List<DraftCookingStep>(),
                    DraftRecipeIngredients = new List<DraftRecipeIngredient>()
                });

            ImageRepositoryMock
                .Setup(r => r.MarkDeletedAsync(It.IsAny<Guid?>()))
                .Returns(Task.CompletedTask);

            ImageRepositoryMock
                .Setup(r => r.MarkDeletedStepsImageFromDraftAsync(It.IsAny<DraftRecipe>()))
                .Returns(Task.CompletedTask);

            DraftRecipeRepositoryMock
                .Setup(r => r.DeleteAsync(It.IsAny<DraftRecipe>()))
                .Returns(Task.CompletedTask);
        }

        [Fact]
        public async Task ShouldThrow_WhenStepOrderDuplicate()
        {
            var req = BuildValidRequest();
            req.CookingSteps.Add(new DraftCookingStepRequest
            {
                StepOrder = 1,
                Instruction = "Y"
            });

            await Assert.ThrowsAsync<AppException>(() => Sut.UpdateDraftAsync(userId, draftId, req));
        }

        [Fact]
        public async Task ShouldThrow_WhenImageOrderDuplicate()
        {
            var req = BuildValidRequest();
            req.CookingSteps[0].Images.Add(new DraftCookingStepImageRequest
            {
                ImageOrder = 1,
                Image = new FileUploadModel
                {
                    FileName = "b.png",
                    Content = new MemoryStream(new byte[1]),
                    ContentType = "image/png"
                }
            });

            await Assert.ThrowsAsync<AppException>(() => Sut.UpdateDraftAsync(userId, draftId, req));
        }

        [Fact]
        public async Task ShouldThrow_WhenIngredientNotExist()
        {
            var req = BuildValidRequest();

            IngredientRepositoryMock
                .Setup(r => r.IdsExistAsync(It.IsAny<List<Guid>>()))
                .ReturnsAsync(false);

            await Assert.ThrowsAsync<AppException>(() => Sut.UpdateDraftAsync(userId, draftId, req));
        }

        [Fact]
        public async Task ShouldThrow_WhenIngredientDuplicate()
        {
            var req = BuildValidRequest();
            var id = req.Ingredients[0].IngredientId;
            req.Ingredients.Add(new DraftRecipeIngredientRequest
            {
                IngredientId = id,
                QuantityGram = 20
            });

            IngredientRepositoryMock
                .Setup(r => r.IdsExistAsync(It.IsAny<List<Guid>>()))
                .ReturnsAsync(true);

            await Assert.ThrowsAsync<AppException>(() => Sut.UpdateDraftAsync(userId, draftId, req));
        }

        [Fact]
        public async Task ShouldThrow_WhenLabelNotExist()
        {
            var req = BuildValidRequest();

            IngredientRepositoryMock
                .Setup(r => r.IdsExistAsync(It.IsAny<List<Guid>>()))
                .ReturnsAsync(true);

            LabelRepositoryMock
                .Setup(r => r.IdsExistAsync(It.IsAny<List<Guid>>()))
                .ReturnsAsync(false);

            await Assert.ThrowsAsync<AppException>(() => Sut.UpdateDraftAsync(userId, draftId, req));
        }

        [Fact]
        public async Task ShouldThrow_WhenLabelDuplicate()
        {
            var req = BuildValidRequest();
            req.LabelIds.Add(req.LabelIds[0]);

            IngredientRepositoryMock
                .Setup(r => r.IdsExistAsync(It.IsAny<List<Guid>>()))
                .ReturnsAsync(true);

            LabelRepositoryMock
                .Setup(r => r.IdsExistAsync(It.IsAny<List<Guid>>()))
                .ReturnsAsync(true);

            await Assert.ThrowsAsync<AppException>(() => Sut.UpdateDraftAsync(userId, draftId, req));
        }

        [Fact]
        public async Task ShouldThrow_WhenDraftNotFound()
        {
            var req = BuildValidRequest();
            SetupValidIngredientLabel(req);

            DraftRecipeRepositoryMock
                .Setup(r => r.GetByIdAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<Func<IQueryable<DraftRecipe>, IQueryable<DraftRecipe>>>()))
                .ReturnsAsync((DraftRecipe)null!);

            await Assert.ThrowsAsync<AppException>(() => Sut.UpdateDraftAsync(userId, draftId, req));
        }

        [Fact]
        public async Task ShouldThrow_WhenNotOwner()
        {
            var req = BuildValidRequest();
            SetupValidIngredientLabel(req);

            DraftRecipeRepositoryMock
                .Setup(r => r.GetByIdAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<Func<IQueryable<DraftRecipe>, IQueryable<DraftRecipe>>>()))
                .ReturnsAsync(new DraftRecipe
                {
                    Id = draftId,
                    AuthorId = Guid.NewGuid()
                });

            await Assert.ThrowsAsync<AppException>(() => Sut.UpdateDraftAsync(userId, draftId, req));
        }

        [Fact]
        public async Task ShouldThrow_WhenTagUserNotExist()
        {
            var req = BuildValidRequest();
            req.TaggedUserIds.Add(Guid.NewGuid());

            SetupValidIngredientLabel(req);
            SetupDraft();

            UserRepositoryMock
                .Setup(r => r.ExistsAsync(It.IsAny<Expression<Func<AppUser, bool>>>()))
                .ReturnsAsync(false);

            await Assert.ThrowsAsync<AppException>(() => Sut.UpdateDraftAsync(userId, draftId, req));
        }

        [Fact]
        public async Task ShouldThrow_WhenSelfTag()
        {
            var req = BuildValidRequest();
            req.TaggedUserIds.Add(userId);

            SetupValidIngredientLabel(req);
            SetupDraft();

            UserRepositoryMock
                .Setup(r => r.ExistsAsync(It.IsAny<Expression<Func<AppUser, bool>>>()))
                .ReturnsAsync(true);

            await Assert.ThrowsAsync<AppException>(() => Sut.UpdateDraftAsync(userId, draftId, req));
        }

        [Fact]
        public async Task ShouldThrow_WhenImageUploadFails()
        {
            var req = BuildValidRequest();
            SetupValidIngredientLabel(req);
            SetupDraft();

            S3Mock
                .Setup(s => s.UploadImageAsync(
                    It.IsAny<FileUploadModel>(),
                    It.IsAny<StorageFolder>(),
                    userId))
                .ThrowsAsync(new AppException(AppResponseCode.SERVICE_NOT_AVAILABLE));

            UserRepositoryMock
                .Setup(r => r.ExistsAsync(It.IsAny<Expression<Func<AppUser, bool>>>()))
                .ReturnsAsync(true);

            await Assert.ThrowsAsync<AppException>(() => Sut.UpdateDraftAsync(userId, draftId, req));
        }

        [Fact]
        public async Task ShouldUpdate_WhenValid()
        {
            var req = BuildValidRequest();

            SetupValidIngredientLabel(req);
            SetupDraft();

            UserRepositoryMock
                .Setup(r => r.ExistsAsync(It.IsAny<Expression<Func<AppUser, bool>>>()))
                .ReturnsAsync(true);

            S3Mock
                .Setup(s => s.UploadImageAsync(
                    It.IsAny<FileUploadModel>(),
                    It.IsAny<StorageFolder>(),
                    userId))
                .ReturnsAsync(new Image
                {
                    Id = Guid.NewGuid(),
                    Key = "x",
                    ContentType = "image/png",
                    CreatedAtUTC = DateTime.UtcNow
                });

            DraftRecipeRepositoryMock
                .Setup(r => r.AddAsync(It.IsAny<DraftRecipe>()))
                .ReturnsAsync(new DraftRecipe { Id = Guid.NewGuid() });

            await Sut.UpdateDraftAsync(userId, draftId, req);

            DraftRecipeRepositoryMock.Verify(r => r.DeleteAsync(It.IsAny<DraftRecipe>()), Times.Once);
            DraftRecipeRepositoryMock.Verify(r => r.AddAsync(It.IsAny<DraftRecipe>()), Times.Once);
        }
    }
}
