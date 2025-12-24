using Moq;
using SEP490_FTCDHMM_API.Application.Dtos.UserDtos;
using SEP490_FTCDHMM_API.Domain.Entities;
using SEP490_FTCDHMM_API.Domain.ValueObjects;

namespace SEP490_FTCDHMM_API.Tests.Services.LabelServiceTests.UserServiceTests
{
    public class ChangeActivityLevelAsyncTests : UserServiceTestBase
    {
        [Fact]
        public async Task ChangeActivityLevel_ShouldUpdateActivityLevel_WhenUserExists()
        {
            var userId = Guid.NewGuid();
            var user = CreateUser(userId, ActivityLevel.From("SEDENTARY"));

            var request = new ChangeActivityLevelRequest
            {
                ActivityLevel = "ACTIVE"
            };

            UserRepositoryMock
                .Setup(r => r.GetByIdAsync(userId, It.IsAny<Func<IQueryable<AppUser>, IQueryable<AppUser>>>()))
                .ReturnsAsync(user);

            UserRepositoryMock
                .Setup(r => r.UpdateAsync(user))
                .Returns(Task.CompletedTask);

            await Sut.ChangeActivityLevelAsync(userId, request);

            Assert.Equal(ActivityLevel.From(request.ActivityLevel), user.ActivityLevel);
            UserRepositoryMock.Verify(r => r.UpdateAsync(user), Times.Once);
        }
    }
}
