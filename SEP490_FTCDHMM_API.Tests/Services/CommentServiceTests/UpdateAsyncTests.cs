using Moq;
using SEP490_FTCDHMM_API.Application.Dtos.CommentDtos;
using SEP490_FTCDHMM_API.Domain.Entities;
using SEP490_FTCDHMM_API.Shared.Exceptions;

namespace SEP490_FTCDHMM_API.Tests.Services.CommentServiceTests
{
    public class UpdateAsyncTests : CommentServiceTestBase
    {
        [Fact]
        public async Task UpdateAsync_ShouldThrow_WhenNotFound()
        {
            CommentRepositoryMock.Setup(r =>
                r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<Func<IQueryable<Comment>, IQueryable<Comment>>>()))
                .ReturnsAsync((Comment?)null);

            await Assert.ThrowsAsync<AppException>(() =>
                Sut.UpdateAsync(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), new UpdateCommentRequest()));
        }

        [Fact]
        public async Task UpdateAsync_ShouldThrow_WhenWrongRecipe()
        {
            var comment = CreateComment();
            CommentRepositoryMock.Setup(r =>
                r.GetByIdAsync(comment.Id, It.IsAny<Func<IQueryable<Comment>, IQueryable<Comment>>>()))
                .ReturnsAsync(comment);

            await Assert.ThrowsAsync<AppException>(() =>
                Sut.UpdateAsync(comment.UserId, Guid.NewGuid(), comment.Id, new UpdateCommentRequest()));
        }

        [Fact]
        public async Task UpdateAsync_ShouldThrow_WhenNotOwner()
        {
            var comment = CreateComment();
            var userId = Guid.NewGuid();
            CommentRepositoryMock.Setup(r =>
                r.GetByIdAsync(comment.Id, It.IsAny<Func<IQueryable<Comment>, IQueryable<Comment>>>()))
                .ReturnsAsync(comment);

            await Assert.ThrowsAsync<AppException>(() =>
                Sut.UpdateAsync(userId, comment.RecipeId, comment.Id, new UpdateCommentRequest()));
        }

        [Fact]
        public async Task UpdateAsync_ShouldThrow_WhenMentionSelf()
        {
            var userId = Guid.NewGuid();
            var comment = CreateComment(userId: userId);
            CommentRepositoryMock.Setup(r =>
                r.GetByIdAsync(comment.Id, It.IsAny<Func<IQueryable<Comment>, IQueryable<Comment>>>()))
                .ReturnsAsync(comment);

            var req = new UpdateCommentRequest
            {
                Content = "hi",
                MentionedUserIds = new List<Guid> { userId }
            };

            await Assert.ThrowsAsync<AppException>(() =>
                Sut.UpdateAsync(userId, comment.RecipeId, comment.Id, req));
        }

        [Fact]
        public async Task UpdateAsync_ShouldUpdate_WhenValid()
        {
            var userId = Guid.NewGuid();
            var recipeId = Guid.NewGuid();
            var comment = CreateComment(userId: userId, recipeId: recipeId);

            CommentRepositoryMock.Setup(r =>
                r.GetByIdAsync(comment.Id, It.IsAny<Func<IQueryable<Comment>, IQueryable<Comment>>>()))
                .ReturnsAsync(comment);

            CommentRepositoryMock.Setup(r => r.UpdateAsync(comment)).Returns(Task.CompletedTask);
            CommentRepositoryMock.Setup(r =>
                r.GetByIdAsync(comment.Id, It.IsAny<Func<IQueryable<Comment>, IQueryable<Comment>>>()))
                .ReturnsAsync(comment);

            await Sut.UpdateAsync(userId, recipeId, comment.Id, new UpdateCommentRequest { Content = "Updated" });

            CommentRepositoryMock.Verify(r => r.UpdateAsync(comment), Times.Once);
            NotifierMock.Verify(n => n.SendCommentUpdatedAsync(recipeId, It.IsAny<CommentResponse>()), Times.Once);
        }
    }
}
