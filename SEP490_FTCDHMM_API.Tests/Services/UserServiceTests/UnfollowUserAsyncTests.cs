using System.Linq.Expressions;
using Moq;
using SEP490_FTCDHMM_API.Domain.Entities;
using SEP490_FTCDHMM_API.Shared.Exceptions;

namespace SEP490_FTCDHMM_API.Tests.Services.UserServiceTests
{
    public class UnfollowUserAsyncTests : UserServiceTestBase
    {
        [Fact]
        public async Task UnfollowUser_ShouldThrow_WhenNotFollowing()
        {
            var followerId = Guid.NewGuid();
            var followeeId = Guid.NewGuid();

            UserFollowRepositoryMock
                .Setup(r => r.GetAllAsync(
                    It.IsAny<Expression<Func<UserFollow, bool>>>(),
                    null
                ))
                .ReturnsAsync(new List<UserFollow>());

            var ex = await Assert.ThrowsAsync<AppException>(() =>
                Sut.UnfollowUserAsync(followerId, followeeId));

            Assert.Equal(AppResponseCode.INVALID_ACTION, ex.ResponseCode);

            UserFollowRepositoryMock.Verify(
                r => r.DeleteAsync(It.IsAny<UserFollow>()),
                Times.Never);
        }

        [Fact]
        public async Task UnfollowUser_ShouldDeleteFollow_WhenExists()
        {
            var followerId = Guid.NewGuid();
            var followeeId = Guid.NewGuid();

            var follow = new UserFollow
            {
                FollowerId = followerId,
                FolloweeId = followeeId,
                CreatedAtUtc = DateTime.UtcNow
            };

            UserFollowRepositoryMock
                .Setup(r => r.GetAllAsync(
                    It.IsAny<Expression<Func<UserFollow, bool>>>(),
                    null
                ))
                .ReturnsAsync(new List<UserFollow> { follow });

            UserFollowRepositoryMock
                .Setup(r => r.DeleteAsync(follow))
                .Returns(Task.CompletedTask)
                .Verifiable();

            await Sut.UnfollowUserAsync(followerId, followeeId);

            UserFollowRepositoryMock.Verify(r => r.DeleteAsync(follow), Times.Once);
        }
    }
}
