using System.Linq.Expressions;
using Moq;
using SEP490_FTCDHMM_API.Application.Dtos.DraftRecipeDtos.Response;
using SEP490_FTCDHMM_API.Domain.Entities;

namespace SEP490_FTCDHMM_API.Tests.Services.DraftRecipeServiceTests
{
    public class GetDraftsAsyncTests : DraftRecipeServiceTestBase
    {
        [Fact]
        public async Task GetDraftsAsync_ShouldReturnMappedResult()
        {
            var userId = NewId();

            var drafts = new List<DraftRecipe>
            {
                new DraftRecipe { Id = NewId(), AuthorId = userId },
                new DraftRecipe { Id = NewId(), AuthorId = userId }
            };

            DraftRecipeRepositoryMock
                .Setup(r => r.GetAllAsync(
                    It.IsAny<Expression<Func<DraftRecipe, bool>>>(),
                    It.IsAny<Func<IQueryable<DraftRecipe>, IQueryable<DraftRecipe>>>()))
                .ReturnsAsync(drafts);

            var mapped = new List<DraftRecipeResponse>
            {
                new DraftRecipeResponse { Name = "A", Difficulty = "EASY" },
                new DraftRecipeResponse { Name = "A", Difficulty = "EASY" }
            };

            MapperMock
                .Setup(m => m.Map<IEnumerable<DraftRecipeResponse>>(drafts))
                .Returns(mapped);

            var result = await Sut.GetDraftsAsync(userId);

            Assert.NotNull(result);
            Assert.Equal(2, result.Count());

            DraftRecipeRepositoryMock.Verify(r =>
                r.GetAllAsync(
                    It.IsAny<Expression<Func<DraftRecipe, bool>>>(),
                    It.IsAny<Func<IQueryable<DraftRecipe>, IQueryable<DraftRecipe>>>()),
                Times.Once);

            MapperMock.Verify(m => m.Map<IEnumerable<DraftRecipeResponse>>(drafts), Times.Once);
        }
    }
}
