using Moq;
using SEP490_FTCDHMM_API.Domain.Entities;
using SEP490_FTCDHMM_API.Shared.Exceptions;

namespace SEP490_FTCDHMM_API.Tests.Services.LabelServiceTests.UserServiceTests
{
    public class FollowUserAsyncTests : UserServiceTestBase
    {
        [Fact]
        public async Task FollowUser_ShouldThrow_WhenFollowerEqualsFollowee()
        {
            var userId = Guid.NewGuid();

            var ex = await Assert.ThrowsAsync<AppException>(() =>
                Sut.FollowUserAsync(userId, userId));

            Assert.Equal(AppResponseCode.INVALID_ACTION, ex.ResponseCode);
        }

        [Fact]
        public async Task FollowUser_ShouldThrow_WhenFolloweeNotFound()
        {
            var followerId = Guid.NewGuid();
            var followeeId = Guid.NewGuid();

            UserRepositoryMock
                .Setup(r => r.GetByIdAsync(
                    followeeId,
                    It.IsAny<Func<IQueryable<AppUser>, IQueryable<AppUser>>?>()
                ))
                .ReturnsAsync((AppUser?)null);

            var ex = await Assert.ThrowsAsync<AppException>(() =>
                Sut.FollowUserAsync(followerId, followeeId));

            Assert.Equal(AppResponseCode.INVALID_ACCOUNT_INFORMATION, ex.ResponseCode);
        }

        [Fact]
        public async Task FollowUser_ShouldThrow_WhenAlreadyFollowing()
        {
            var followerId = Guid.NewGuid();
            var followeeId = Guid.NewGuid();

            UserRepositoryMock
                .Setup(r => r.GetByIdAsync(
                    followeeId,
                    It.IsAny<Func<IQueryable<AppUser>, IQueryable<AppUser>>?>()
                ))
                .ReturnsAsync(CreateUser(followeeId));

            UserFollowRepositoryMock
                .Setup(r => r.ExistsAsync(
                    It.IsAny<System.Linq.Expressions.Expression<Func<UserFollow, bool>>>()))
                .ReturnsAsync(true);

            var ex = await Assert.ThrowsAsync<AppException>(() =>
                Sut.FollowUserAsync(followerId, followeeId));

            Assert.Equal(AppResponseCode.INVALID_ACTION, ex.ResponseCode);
        }

        [Fact]
        public async Task FollowUser_ShouldAddFollow_WhenValid()
        {
            var followerId = Guid.NewGuid();
            var followeeId = Guid.NewGuid();

            UserRepositoryMock
                .Setup(r => r.GetByIdAsync(
                    followeeId,
                    It.IsAny<Func<IQueryable<AppUser>, IQueryable<AppUser>>?>()
                ))
                .ReturnsAsync(CreateUser(followeeId));

            UserFollowRepositoryMock
                .Setup(r => r.ExistsAsync(
                    It.IsAny<System.Linq.Expressions.Expression<Func<UserFollow, bool>>>()))
                .ReturnsAsync(false);

            UserFollow? savedFollow = null;

            UserFollowRepositoryMock
                .Setup(r => r.AddAsync(It.IsAny<UserFollow>()))
                .Callback<UserFollow>(f => savedFollow = f)
                .ReturnsAsync((UserFollow f) => f);

            await Sut.FollowUserAsync(followerId, followeeId);

            Assert.NotNull(savedFollow);
            Assert.Equal(followerId, savedFollow!.FollowerId);
            Assert.Equal(followeeId, savedFollow.FolloweeId);
            Assert.True(savedFollow.CreatedAtUtc <= DateTime.UtcNow);
        }
    }
}
