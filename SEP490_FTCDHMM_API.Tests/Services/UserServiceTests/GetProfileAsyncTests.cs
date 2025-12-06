using System.Linq.Expressions;
using Moq;
using SEP490_FTCDHMM_API.Application.Dtos.UserDtos;
using SEP490_FTCDHMM_API.Domain.Entities;
using SEP490_FTCDHMM_API.Shared.Exceptions;

namespace SEP490_FTCDHMM_API.Tests.Services.UserServiceTests
{
    public class GetProfileAsyncTests : UserServiceTestBase
    {
        private ProfileResponse MockProfile(AppUser user)
        {
            return new ProfileResponse
            {
                UserName = user.UserName!,
                Email = user.Email!,
                FirstName = user.FirstName,
                LastName = user.LastName
            };
        }

        [Fact]
        public async Task GetProfile_ShouldThrow_WhenUserNotFound()
        {
            UserRepositoryMock
                .Setup(r => r.FirstOrDefaultAsync(
                    It.IsAny<Expression<Func<AppUser, string>>>(),
                    It.IsAny<Expression<Func<AppUser, bool>>>(),
                    It.IsAny<Func<IQueryable<AppUser>, IQueryable<AppUser>>>()))
                .ReturnsAsync((AppUser)null!);

            var ex = await Assert.ThrowsAsync<AppException>(() => Sut.GetProfileAsync("unknown"));

            Assert.Equal(AppResponseCode.INVALID_ACCOUNT_INFORMATION, ex.ResponseCode);
        }

        [Fact]
        public async Task GetProfile_ShouldReturnMappedProfile()
        {
            var user = CreateUser();
            var mapped = MockProfile(user);

            UserRepositoryMock
                .Setup(r => r.FirstOrDefaultAsync(
                    It.IsAny<Expression<Func<AppUser, string>>>(),
                    It.IsAny<Expression<Func<AppUser, bool>>>(),
                    It.IsAny<Func<IQueryable<AppUser>, IQueryable<AppUser>>>()))
                .ReturnsAsync(user);

            MapperMock.Setup(m => m.Map<ProfileResponse>(user))
                      .Returns(mapped);

            UserFollowRepositoryMock.Setup(f => f.CountAsync(It.IsAny<Expression<Func<UserFollow, bool>>>()))
                                    .ReturnsAsync(10);

            UserFollowRepositoryMock.Setup(f => f.CountAsync(It.IsAny<Expression<Func<UserFollow, bool>>>()))
                                    .ReturnsAsync(5);

            var result = await Sut.GetProfileAsync(user.UserName);

            Assert.Equal(user.UserName, result.UserName);
        }

        [Fact]
        public async Task GetProfile_ShouldSetIsFollowingFalse_WhenCurrentUserIdIsNull()
        {
            var user = CreateUser();
            var mapped = MockProfile(user);

            UserRepositoryMock
                .Setup(r => r.FirstOrDefaultAsync(
                    It.IsAny<Expression<Func<AppUser, string>>>(),
                    It.IsAny<Expression<Func<AppUser, bool>>>(),
                    It.IsAny<Func<IQueryable<AppUser>, IQueryable<AppUser>>>()))
                .ReturnsAsync(user);

            MapperMock.Setup(m => m.Map<ProfileResponse>(user))
                      .Returns(mapped);

            var result = await Sut.GetProfileAsync(user.UserName, null);

            Assert.False(result.IsFollowing);
        }

        [Fact]
        public async Task GetProfile_ShouldSetIsFollowingFalse_WhenCurrentUserIdEqualsProfileUserId()
        {
            var user = CreateUser();
            var mapped = MockProfile(user);

            UserRepositoryMock
                .Setup(r => r.FirstOrDefaultAsync(
                    It.IsAny<Expression<Func<AppUser, string>>>(),
                    It.IsAny<Expression<Func<AppUser, bool>>>(),
                    It.IsAny<Func<IQueryable<AppUser>, IQueryable<AppUser>>>()))
                .ReturnsAsync(user);

            MapperMock.Setup(m => m.Map<ProfileResponse>(user))
                      .Returns(mapped);

            var result = await Sut.GetProfileAsync(user.UserName, user.Id);

            Assert.False(result.IsFollowing);
        }

        [Fact]
        public async Task GetProfile_ShouldSetIsFollowingTrue_WhenCurrentUserFollowsTarget()
        {
            var user = CreateUser();
            var mapped = MockProfile(user);
            var currentUserId = Guid.NewGuid();

            UserRepositoryMock
                .Setup(r => r.FirstOrDefaultAsync(
                    It.IsAny<Expression<Func<AppUser, string>>>(),
                    It.IsAny<Expression<Func<AppUser, bool>>>(),
                    It.IsAny<Func<IQueryable<AppUser>, IQueryable<AppUser>>>()))
                .ReturnsAsync(user);

            MapperMock.Setup(m => m.Map<ProfileResponse>(user))
                      .Returns(mapped);

            UserFollowRepositoryMock
                .Setup(f => f.ExistsAsync(It.IsAny<Expression<Func<UserFollow, bool>>>()))
                .ReturnsAsync(true);

            var result = await Sut.GetProfileAsync(user.UserName, currentUserId);

            Assert.True(result.IsFollowing);
        }

        [Fact]
        public async Task GetProfile_ShouldSetIsFollowingFalse_WhenCurrentUserDoesNotFollowTarget()
        {
            var user = CreateUser();
            var mapped = MockProfile(user);
            var currentUserId = Guid.NewGuid();

            UserRepositoryMock
                .Setup(r => r.FirstOrDefaultAsync(
                    It.IsAny<Expression<Func<AppUser, string>>>(),
                    It.IsAny<Expression<Func<AppUser, bool>>>(),
                    It.IsAny<Func<IQueryable<AppUser>, IQueryable<AppUser>>>()))
                .ReturnsAsync(user);

            MapperMock.Setup(m => m.Map<ProfileResponse>(user))
                      .Returns(mapped);

            UserFollowRepositoryMock
                .Setup(f => f.ExistsAsync(It.IsAny<Expression<Func<UserFollow, bool>>>()))
                .ReturnsAsync(false);

            var result = await Sut.GetProfileAsync(user.UserName, currentUserId);

            Assert.False(result.IsFollowing);
        }
    }
}
