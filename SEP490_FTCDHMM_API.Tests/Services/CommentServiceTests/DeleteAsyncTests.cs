using Moq;
using SEP490_FTCDHMM_API.Domain.Entities;
using SEP490_FTCDHMM_API.Domain.ValueObjects;
using SEP490_FTCDHMM_API.Shared.Exceptions;

namespace SEP490_FTCDHMM_API.Tests.Services.CommentServiceTests
{
    public class DeleteAsyncTests : CommentServiceTestBase
    {
        [Fact]
        public async Task DeleteAsync_ShouldThrow_WhenCommentNotFound()
        {
            CommentRepositoryMock.Setup(r =>
                r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<Func<IQueryable<Comment>, IQueryable<Comment>>>()))
                .ReturnsAsync((Comment?)null);

            await Assert.ThrowsAsync<AppException>(() =>
                Sut.DeleteAsync(Guid.NewGuid(), Guid.NewGuid(), DeleteMode.Self));
        }

        [Fact]
        public async Task DeleteAsync_ShouldThrow_WhenNotOwner_SelfMode()
        {
            var comment = CreateComment(userId: Guid.NewGuid());
            CommentRepositoryMock.Setup(r =>
                r.GetByIdAsync(comment.Id, It.IsAny<Func<IQueryable<Comment>, IQueryable<Comment>>>()))
                .ReturnsAsync(comment);

            await Assert.ThrowsAsync<AppException>(() =>
                Sut.DeleteAsync(Guid.NewGuid(), comment.Id, DeleteMode.Self));
        }

        [Fact]
        public async Task DeleteAsync_ShouldDelete_WhenValidSelf()
        {
            var userId = Guid.NewGuid();
            var comment = CreateComment(userId: userId);
            CommentRepositoryMock.Setup(r =>
                r.GetByIdAsync(comment.Id, It.IsAny<Func<IQueryable<Comment>, IQueryable<Comment>>>()))
                .ReturnsAsync(comment);

            await Sut.DeleteAsync(userId, comment.Id, DeleteMode.Self);

            CommentRepositoryMock.Verify(r => r.DeleteAsync(comment), Times.Once);
            NotifierMock.Verify(n => n.SendCommentDeletedAsync(comment.RecipeId, comment.Id), Times.Once);
        }
    }
}
