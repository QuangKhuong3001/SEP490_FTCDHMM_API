using System.Linq.Expressions;
using Moq;
using SEP490_FTCDHMM_API.Application.Dtos.CommentDtos;
using SEP490_FTCDHMM_API.Application.Dtos.CommentDtos.CommentMention;
using SEP490_FTCDHMM_API.Domain.Entities;

namespace SEP490_FTCDHMM_API.Tests.Services.CommentServiceTests
{
    public class GetAllCommentByRecipeAsyncTests : CommentServiceTestBase
    {
        [Fact]
        public async Task GetAllByRecipe_ShouldReturnList()
        {
            var recipeId = Guid.NewGuid();

            var rootComment = CreateComment(recipeId: recipeId, parentId: null);
            var reply = CreateComment(recipeId: recipeId, parentId: rootComment.Id);

            CommentRepositoryMock
                .SetupSequence(r => r.GetAllAsync(
                    It.IsAny<Expression<Func<Comment, bool>>>(),
                    It.IsAny<Func<IQueryable<Comment>, IQueryable<Comment>>>()))
                .ReturnsAsync(new List<Comment> { rootComment })
                .ReturnsAsync(new List<Comment> { reply });

            CommentMentionRepositoryMock
                .Setup(r => r.GetAllAsync(
                    It.IsAny<Expression<Func<CommentMention, bool>>>(),
                    It.IsAny<Func<IQueryable<CommentMention>, IQueryable<CommentMention>>>()))
                .ReturnsAsync(new List<CommentMention>());

            MapperMock
                .Setup(m => m.Map<List<CommentResponse>>(It.IsAny<IEnumerable<Comment>>()))
                .Returns<IEnumerable<Comment>>(src =>
                    src.Select(c => new CommentResponse
                    {
                        Id = c.Id,
                        ParentCommentId = c.ParentCommentId,
                        Replies = new List<CommentResponse>()
                    }).ToList());

            MapperMock
                .Setup(m => m.Map<List<MentionedUserResponse>>(It.IsAny<IEnumerable<CommentMention>>()))
                .Returns(new List<MentionedUserResponse>());

            var result = await Sut.GetCommentsByRecipeAsync(recipeId);

            Assert.Single(result);
            Assert.Equal(rootComment.Id, result[0].Id);
        }

    }
}
