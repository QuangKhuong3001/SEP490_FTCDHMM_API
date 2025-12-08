using Moq;
using SEP490_FTCDHMM_API.Application.Dtos.CommentDtos;
using SEP490_FTCDHMM_API.Domain.Entities;
using SEP490_FTCDHMM_API.Shared.Exceptions;

namespace SEP490_FTCDHMM_API.Tests.Services.CommentServiceTests
{
    public class GetAllCommentByRecipeAsyncTests : CommentServiceTestBase
    {
        [Fact]
        public async Task GetAllByRecipe_ShouldThrow_WhenRecipeNotExist()
        {
            RecipeRepositoryMock.Setup(r => r.ExistsAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Recipe, bool>>>()))
                .ReturnsAsync(false);

            await Assert.ThrowsAsync<AppException>(() => Sut.GetAllCommentByRecipeAsync(Guid.NewGuid()));
        }

        [Fact]
        public async Task GetAllByRecipe_ShouldReturnList()
        {
            RecipeRepositoryMock.Setup(r => r.ExistsAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Recipe, bool>>>()))
                .ReturnsAsync(true);

            CommentRepositoryMock.Setup(r =>
                r.GetAllAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Comment, bool>>>(),
                It.IsAny<Func<IQueryable<Comment>, IQueryable<Comment>>>()))
                .ReturnsAsync(new List<Comment> { CreateComment() });

            MapperMock.Setup(m => m.Map<List<CommentResponse>>(It.IsAny<object>()))
                .Returns(new List<CommentResponse> { new CommentResponse() });

            var result = await Sut.GetAllCommentByRecipeAsync(Guid.NewGuid());
            Assert.Single(result);
        }
    }
}
