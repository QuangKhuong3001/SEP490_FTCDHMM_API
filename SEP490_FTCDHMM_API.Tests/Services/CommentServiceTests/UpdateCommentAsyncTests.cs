using System.Linq.Expressions;
using Moq;
using SEP490_FTCDHMM_API.Application.Dtos.CommentDtos;
using SEP490_FTCDHMM_API.Domain.Entities;
using SEP490_FTCDHMM_API.Domain.ValueObjects;
using SEP490_FTCDHMM_API.Shared.Exceptions;

namespace SEP490_FTCDHMM_API.Tests.Services.CommentServiceTests
{
    public class UpdateCommentAsyncTests : CommentServiceTestBase
    {
        private Recipe CreateRecipe(Guid id, Guid authorId, bool posted)
        {
            var recipe = new Recipe
            {
                Id = id,
                AuthorId = authorId,
                Name = "Recipe name",
                Ration = 1
            };

            recipe.Status = posted ? RecipeStatus.Posted : RecipeStatus.Locked;

            return recipe;
        }

        private Comment CreateComment(Guid id, Guid userId, Guid recipeId)
        {
            return new Comment
            {
                Id = id,
                UserId = userId,
                RecipeId = recipeId,
                Content = "old",
                CreatedAtUtc = DateTime.UtcNow,
                Mentions = new List<CommentMention>()
            };
        }

        [Fact]
        public async Task UpdateAsync_ShouldThrow_WhenCommentNotFound()
        {
            var userId = Guid.NewGuid();
            var recipeId = Guid.NewGuid();
            var commentId = Guid.NewGuid();
            var request = new UpdateCommentRequest();

            CommentRepositoryMock
                .Setup(r => r.GetByIdAsync(commentId,
                    It.IsAny<Func<IQueryable<Comment>, IQueryable<Comment>>>()))
                .ReturnsAsync((Comment?)null);

            await Assert.ThrowsAsync<AppException>(() =>
                Sut.UpdateCommentAsync(userId, recipeId, commentId, request));

            CommentRepositoryMock.Verify(r => r.GetByIdAsync(commentId,
                It.IsAny<Func<IQueryable<Comment>, IQueryable<Comment>>>()), Times.Once);
            RecipeRepositoryMock.Verify(r => r.GetByIdAsync(It.IsAny<Guid>(), null), Times.Never);
            CommentRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Comment>()), Times.Never);
            NotifierMock.Verify(n => n.SendCommentUpdatedAsync(It.IsAny<Guid>(), It.IsAny<object>()), Times.Never);
        }

        [Fact]
        public async Task UpdateAsync_ShouldThrow_WhenRecipeNotFound()
        {
            var userId = Guid.NewGuid();
            var recipeId = Guid.NewGuid();
            var commentId = Guid.NewGuid();
            var comment = CreateComment(commentId, userId, recipeId);
            var request = new UpdateCommentRequest();

            CommentRepositoryMock
                .Setup(r => r.GetByIdAsync(commentId,
                    It.IsAny<Func<IQueryable<Comment>, IQueryable<Comment>>>()))
                .ReturnsAsync(comment);

            RecipeRepositoryMock
                .Setup(r => r.GetByIdAsync(recipeId, null))
                .ReturnsAsync((Recipe?)null);

            await Assert.ThrowsAsync<AppException>(() =>
                Sut.UpdateCommentAsync(userId, recipeId, commentId, request));

            RecipeRepositoryMock.Verify(r => r.GetByIdAsync(recipeId, null), Times.Once);
            CommentRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Comment>()), Times.Never);
            NotifierMock.Verify(n => n.SendCommentUpdatedAsync(It.IsAny<Guid>(), It.IsAny<object>()), Times.Never);
        }

        [Fact]
        public async Task UpdateAsync_ShouldThrow_WhenRecipeNotPosted()
        {
            var userId = Guid.NewGuid();
            var recipeId = Guid.NewGuid();
            var commentId = Guid.NewGuid();
            var authorId = Guid.NewGuid();
            var comment = CreateComment(commentId, userId, recipeId);
            var request = new UpdateCommentRequest();

            var recipe = CreateRecipe(recipeId, authorId, posted: false);

            CommentRepositoryMock
                .Setup(r => r.GetByIdAsync(commentId,
                    It.IsAny<Func<IQueryable<Comment>, IQueryable<Comment>>>()))
                .ReturnsAsync(comment);

            RecipeRepositoryMock
                .Setup(r => r.GetByIdAsync(recipeId, null))
                .ReturnsAsync(recipe);

            await Assert.ThrowsAsync<AppException>(() =>
                Sut.UpdateCommentAsync(userId, recipeId, commentId, request));

            RecipeRepositoryMock.Verify(r => r.GetByIdAsync(recipeId, null), Times.Once);
            CommentRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Comment>()), Times.Never);
            NotifierMock.Verify(n => n.SendCommentUpdatedAsync(It.IsAny<Guid>(), It.IsAny<object>()), Times.Never);
        }

        [Fact]
        public async Task UpdateAsync_ShouldThrow_WhenCommentNotBelongToRecipe()
        {
            var userId = Guid.NewGuid();
            var recipeId = Guid.NewGuid();
            var otherRecipeId = Guid.NewGuid();
            var commentId = Guid.NewGuid();
            var authorId = Guid.NewGuid();

            var comment = CreateComment(commentId, userId, otherRecipeId);
            var recipe = CreateRecipe(recipeId, authorId, posted: true);

            var request = new UpdateCommentRequest();

            CommentRepositoryMock
                .Setup(r => r.GetByIdAsync(commentId,
                    It.IsAny<Func<IQueryable<Comment>, IQueryable<Comment>>>()))
                .ReturnsAsync(comment);

            RecipeRepositoryMock
                .Setup(r => r.GetByIdAsync(recipeId, null))
                .ReturnsAsync(recipe);

            await Assert.ThrowsAsync<AppException>(() =>
                Sut.UpdateCommentAsync(userId, recipeId, commentId, request));

            CommentRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Comment>()), Times.Never);
            NotifierMock.Verify(n => n.SendCommentUpdatedAsync(It.IsAny<Guid>(), It.IsAny<object>()), Times.Never);
        }

        [Fact]
        public async Task UpdateAsync_ShouldThrow_WhenUserIsNotOwner()
        {
            var userId = Guid.NewGuid();
            var otherUserId = Guid.NewGuid();
            var recipeId = Guid.NewGuid();
            var commentId = Guid.NewGuid();
            var authorId = Guid.NewGuid();

            var comment = CreateComment(commentId, otherUserId, recipeId);
            var recipe = CreateRecipe(recipeId, authorId, posted: true);

            var request = new UpdateCommentRequest();

            CommentRepositoryMock
                .Setup(r => r.GetByIdAsync(commentId,
                    It.IsAny<Func<IQueryable<Comment>, IQueryable<Comment>>>()))
                .ReturnsAsync(comment);

            RecipeRepositoryMock
                .Setup(r => r.GetByIdAsync(recipeId, null))
                .ReturnsAsync(recipe);

            await Assert.ThrowsAsync<AppException>(() =>
                Sut.UpdateCommentAsync(userId, recipeId, commentId, request));

            CommentRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Comment>()), Times.Never);
            NotifierMock.Verify(n => n.SendCommentUpdatedAsync(It.IsAny<Guid>(), It.IsAny<object>()), Times.Never);
        }

        [Fact]
        public async Task UpdateAsync_ShouldThrow_WhenMentionedUserDoesNotExist()
        {
            var userId = Guid.NewGuid();
            var recipeId = Guid.NewGuid();
            var commentId = Guid.NewGuid();
            var authorId = Guid.NewGuid();
            var mentionedId = Guid.NewGuid();

            var comment = CreateComment(commentId, userId, recipeId);
            var recipe = CreateRecipe(recipeId, authorId, posted: true);

            var request = new UpdateCommentRequest
            {
                Content = "new",
                MentionedUserIds = new List<Guid> { mentionedId }
            };

            CommentRepositoryMock
                .Setup(r => r.GetByIdAsync(commentId,
                    It.IsAny<Func<IQueryable<Comment>, IQueryable<Comment>>>()))
                .ReturnsAsync(comment);

            RecipeRepositoryMock
                .Setup(r => r.GetByIdAsync(recipeId, null))
                .ReturnsAsync(recipe);

            UserRepositoryMock
                .Setup(r => r.ExistsAsync(It.IsAny<Expression<Func<AppUser, bool>>>()))
                .ReturnsAsync(false);

            await Assert.ThrowsAsync<AppException>(() =>
                Sut.UpdateCommentAsync(userId, recipeId, commentId, request));

            CommentRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Comment>()), Times.Never);
            NotifierMock.Verify(n => n.SendCommentUpdatedAsync(It.IsAny<Guid>(), It.IsAny<object>()), Times.Never);
        }

        [Fact]
        public async Task UpdateAsync_ShouldThrow_WhenUserMentionsThemself()
        {
            var userId = Guid.NewGuid();
            var recipeId = Guid.NewGuid();
            var commentId = Guid.NewGuid();
            var authorId = Guid.NewGuid();

            var comment = CreateComment(commentId, userId, recipeId);
            var recipe = CreateRecipe(recipeId, authorId, posted: true);

            var request = new UpdateCommentRequest
            {
                Content = "new",
                MentionedUserIds = new List<Guid> { userId }
            };

            CommentRepositoryMock
                .Setup(r => r.GetByIdAsync(commentId,
                    It.IsAny<Func<IQueryable<Comment>, IQueryable<Comment>>>()))
                .ReturnsAsync(comment);

            RecipeRepositoryMock
                .Setup(r => r.GetByIdAsync(recipeId, null))
                .ReturnsAsync(recipe);

            UserRepositoryMock
                .Setup(r => r.ExistsAsync(It.IsAny<Expression<Func<AppUser, bool>>>()))
                .ReturnsAsync(true);

            await Assert.ThrowsAsync<AppException>(() =>
                Sut.UpdateCommentAsync(userId, recipeId, commentId, request));

            CommentRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Comment>()), Times.Never);
            NotifierMock.Verify(n => n.SendCommentUpdatedAsync(It.IsAny<Guid>(), It.IsAny<object>()), Times.Never);
        }

        [Fact]
        public async Task UpdateAsync_ShouldUpdate_WhenValid()
        {
            var userId = Guid.NewGuid();
            var recipeId = Guid.NewGuid();
            var commentId = Guid.NewGuid();
            var authorId = Guid.NewGuid();
            var mentionedId1 = Guid.NewGuid();
            var mentionedId2 = Guid.NewGuid();

            var recipe = CreateRecipe(recipeId, authorId, posted: true);

            var comment = CreateComment(commentId, userId, recipeId);
            comment.Mentions.Add(new CommentMention
            {
                CommentId = commentId,
                MentionedUserId = Guid.NewGuid()
            });

            var updatedComment = CreateComment(commentId, userId, recipeId);

            var request = new UpdateCommentRequest
            {
                Content = "updated content",
                MentionedUserIds = new List<Guid> { mentionedId1, mentionedId2, mentionedId1 }
            };

            CommentRepositoryMock
                .SetupSequence(r => r.GetByIdAsync(commentId,
                        It.IsAny<Func<IQueryable<Comment>, IQueryable<Comment>>>()))
                .ReturnsAsync(comment)
                .ReturnsAsync(updatedComment);

            NotificationCommandServiceMock
                .Setup(n => n.CreateAndSendNotificationAsync(
                    userId,
                    It.IsAny<Guid>(),
                    NotificationType.Mention,
                    recipeId))
                .Returns(Task.CompletedTask);

            RecipeRepositoryMock
                .Setup(r => r.GetByIdAsync(recipeId, null))
                .ReturnsAsync(recipe);

            UserRepositoryMock
                .Setup(r => r.ExistsAsync(It.IsAny<Expression<Func<AppUser, bool>>>()))
                .ReturnsAsync(true);

            CommentRepositoryMock
                .Setup(r => r.UpdateAsync(comment))
                .Returns(Task.CompletedTask);

            MapperMock
                .Setup(m => m.Map<CommentResponse>(updatedComment))
                .Returns(new CommentResponse());

            NotifierMock
                .Setup(n => n.SendCommentUpdatedAsync(recipeId, It.IsAny<object>()))
                .Returns(Task.CompletedTask);

            await Sut.UpdateCommentAsync(userId, recipeId, commentId, request);

            CommentRepositoryMock.Verify(r => r.GetByIdAsync(commentId,
                It.IsAny<Func<IQueryable<Comment>, IQueryable<Comment>>>()), Times.Exactly(2));
            RecipeRepositoryMock.Verify(r => r.GetByIdAsync(recipeId, null), Times.Once);
            UserRepositoryMock.Verify(r => r.ExistsAsync(It.IsAny<Expression<Func<AppUser, bool>>>()),
                Times.AtLeastOnce);
            CommentRepositoryMock.Verify(r => r.UpdateAsync(comment), Times.Once);
            MapperMock.Verify(m => m.Map<CommentResponse>(updatedComment), Times.Once);
            NotifierMock.Verify(n => n.SendCommentUpdatedAsync(recipeId, It.IsAny<object>()), Times.Once);

            Assert.Equal("updated content", comment.Content);
            Assert.Equal(2, comment.Mentions.Count);
            Assert.True(comment.Mentions.All(m => m.CommentId == commentId));
            var distinctIds = comment.Mentions.Select(m => m.MentionedUserId).Distinct().ToList();
            Assert.Equal(2, distinctIds.Count);
            Assert.Contains(mentionedId1, distinctIds);
            Assert.Contains(mentionedId2, distinctIds);
        }
    }
}
