using Moq;
using SEP490_FTCDHMM_API.Domain.Entities;
using SEP490_FTCDHMM_API.Domain.ValueObjects;
using SEP490_FTCDHMM_API.Shared.Exceptions;

namespace SEP490_FTCDHMM_API.Tests.Services.UserServiceTests
{
    public class GetActivityLevelAsyncTests : UserServiceTestBase
    {
        [Fact]
        public async Task GetActivityLevel_ShouldThrow_WhenUserNotFound()
        {
            var userId = Guid.NewGuid();

            UserRepositoryMock
                .Setup(r => r.GetByIdAsync(userId, It.IsAny<Func<IQueryable<AppUser>, IQueryable<AppUser>>>()))
                .ReturnsAsync((AppUser)null!);

            var ex = await Assert.ThrowsAsync<AppException>(() => Sut.GetActivityLevelAsync(userId));

            Assert.Equal(AppResponseCode.NOT_FOUND, ex.ResponseCode);
        }

        [Fact]
        public async Task GetActivityLevel_ShouldReturnActivityLevel_WhenUserExists()
        {
            var userId = Guid.NewGuid();
            var activityLevel = ActivityLevel.From("ACTIVE");
            var user = CreateUser(userId, activityLevel);

            UserRepositoryMock
                .Setup(r => r.GetByIdAsync(userId, It.IsAny<Func<IQueryable<AppUser>, IQueryable<AppUser>>>()))
                .ReturnsAsync(user);

            var result = await Sut.GetActivityLevelAsync(userId);

            Assert.Equal(activityLevel, result);
            UserRepositoryMock.Verify(r => r.GetByIdAsync(userId, It.IsAny<Func<IQueryable<AppUser>, IQueryable<AppUser>>>()), Times.Once);
        }
    }
}
