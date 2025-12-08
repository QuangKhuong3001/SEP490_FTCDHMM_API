using Moq;
using SEP490_FTCDHMM_API.Application.Dtos.DraftRecipeDtos.Response;
using SEP490_FTCDHMM_API.Domain.Entities;
using SEP490_FTCDHMM_API.Shared.Exceptions;

namespace SEP490_FTCDHMM_API.Tests.Services.DraftRecipeServiceTests
{
    public class GetDraftByIdAsyncTests : DraftRecipeServiceTestBase
    {
        [Fact]
        public async Task GetDraftById_ShouldThrow_WhenDraftNotFound()
        {
            DraftRecipeRepositoryMock
                .Setup(r => r.GetByIdAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<Func<IQueryable<DraftRecipe>, IQueryable<DraftRecipe>>>()))
                .ReturnsAsync((DraftRecipe)null!);

            await Assert.ThrowsAsync<AppException>(() =>
                Sut.GetDraftByIdAsync(NewId(), NewId()));
        }

        [Fact]
        public async Task GetDraftById_ShouldThrow_WhenNotOwner()
        {
            var draft = new DraftRecipe { AuthorId = Guid.NewGuid() };

            DraftRecipeRepositoryMock
                .Setup(r => r.GetByIdAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<Func<IQueryable<DraftRecipe>, IQueryable<DraftRecipe>>>()))
                .ReturnsAsync(draft);

            await Assert.ThrowsAsync<AppException>(() =>
                Sut.GetDraftByIdAsync(NewId(), NewId()));
        }

        [Fact]
        public async Task GetDraftById_ShouldReturnMappedResult()
        {
            var userId = NewId();

            var draft = new DraftRecipe
            {
                Id = Guid.NewGuid(),
                AuthorId = userId
            };

            DraftRecipeRepositoryMock
                .Setup(r => r.GetByIdAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<Func<IQueryable<DraftRecipe>, IQueryable<DraftRecipe>>>()))
                .ReturnsAsync(draft);

            var mapped = new DraftDetailsResponse
            {
                Name = "A",
                Difficulty = "EASY"
            };

            MapperMock
                .Setup(m => m.Map<DraftDetailsResponse>(draft))
                .Returns(mapped);

            var result = await Sut.GetDraftByIdAsync(userId, Guid.NewGuid());

            Assert.NotNull(result);
            Assert.Equal("A", result.Name);

            DraftRecipeRepositoryMock.VerifyAll();
            MapperMock.VerifyAll();
        }
    }
}
