using Moq;
using SEP490_FTCDHMM_API.Domain.Entities;
using SEP490_FTCDHMM_API.Shared.Exceptions;

namespace SEP490_FTCDHMM_API.Tests.Services.UserHealthGoalServiceTests
{
    public class RemoveFromCurrentAsyncTests : UserHealthGoalServiceTestBase
    {
        [Fact]
        public async Task Remove_ShouldThrow_WhenNoCurrentGoal()
        {
            var userId = NewId();

            UserHealthGoalRepositoryMock
                .Setup(r => r.GetActiveGoalByUserIdAsync(userId))
                .ReturnsAsync((UserHealthGoal)null!);

            await Assert.ThrowsAsync<AppException>(() =>
                Sut.RemoveGoalFromCurrentAsync(userId));

            UserHealthGoalRepositoryMock.VerifyAll();
        }

        [Fact]
        public async Task Remove_ShouldSucceed_WhenHasActiveGoal()
        {
            var userId = NewId();
            var goal = new UserHealthGoal { UserId = userId, ExpiredAtUtc = null };

            UserHealthGoalRepositoryMock
                .Setup(r => r.GetActiveGoalByUserIdAsync(userId))
                .ReturnsAsync(goal);

            UserHealthGoalRepositoryMock
                .Setup(r => r.UpdateAsync(It.Is<UserHealthGoal>(x =>
                    x.ExpiredAtUtc != null)))
                .Returns(Task.CompletedTask);

            CacheServiceMock
                .Setup(x => x.RemoveByPrefixAsync(It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            await Sut.RemoveGoalFromCurrentAsync(userId);

            UserHealthGoalRepositoryMock.VerifyAll();
        }
    }
}
