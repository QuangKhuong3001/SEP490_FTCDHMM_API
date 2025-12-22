using Moq;
using SEP490_FTCDHMM_API.Domain.Entities;
using SEP490_FTCDHMM_API.Domain.ValueObjects;
using SEP490_FTCDHMM_API.Shared.Exceptions;

namespace SEP490_FTCDHMM_API.Tests.Services.CommentServiceTests
{
    public class DeleteCommentAsyncTests : CommentServiceTestBase
    {
        [Fact]
        public async Task DeleteAsync_ShouldThrow_WhenCommentNotFound()
        {
            CommentRepositoryMock.Setup(r =>
                r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<Func<IQueryable<Comment>, IQueryable<Comment>>>()))
                .ReturnsAsync((Comment?)null);

            await Assert.ThrowsAsync<AppException>(() =>
                Sut.DeleteCommentAsync(Guid.NewGuid(), Guid.NewGuid(), DeleteMode.Self));
        }

        [Fact]
        public async Task DeleteAsync_ShouldThrow_WhenNotOwner_SelfMode()
        {
            var comment = CreateComment(userId: Guid.NewGuid());
            CommentRepositoryMock.Setup(r =>
                r.GetByIdAsync(comment.Id, It.IsAny<Func<IQueryable<Comment>, IQueryable<Comment>>>()))
                .ReturnsAsync(comment);

            await Assert.ThrowsAsync<AppException>(() =>
                Sut.DeleteCommentAsync(Guid.NewGuid(), comment.Id, DeleteMode.Self));
        }

        [Fact]
        public async Task DeleteAsync_ShouldDelete_WhenValidSelf()
        {
            var userId = Guid.NewGuid();
            var recipeId = Guid.NewGuid();

            var comment = new Comment
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                RecipeId = recipeId,
                Recipe = new Recipe
                {
                    Id = recipeId,
                    AuthorId = Guid.NewGuid()
                },
                Replies = new List<Comment>()
            };

            CommentRepositoryMock
                .Setup(r => r.GetByIdAsync(
                    comment.Id,
                    It.IsAny<Func<IQueryable<Comment>, IQueryable<Comment>>>()))
                .ReturnsAsync(comment);

            CommentRepositoryMock
                .Setup(r => r.DeleteAsync(comment))
                .Returns(Task.CompletedTask);

            NotifierMock
                .Setup(n => n.SendCommentDeletedAsync(recipeId, comment.Id))
                .Returns(Task.CompletedTask);

            await Sut.DeleteCommentAsync(userId, comment.Id, DeleteMode.Self);

            CommentRepositoryMock.Verify(r => r.DeleteAsync(comment), Times.Once);
            NotifierMock.Verify(n => n.SendCommentDeletedAsync(recipeId, comment.Id), Times.Once);
        }

    }
}
