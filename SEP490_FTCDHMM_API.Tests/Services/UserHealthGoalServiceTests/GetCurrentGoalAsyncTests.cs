using Moq;
using SEP490_FTCDHMM_API.Application.Dtos.UserHealthGoalDtos;
using SEP490_FTCDHMM_API.Domain.Entities;

namespace SEP490_FTCDHMM_API.Tests.Services.UserHealthGoalServiceTests
{
    public class GetCurrentGoalAsyncTests : UserHealthGoalServiceTestBase
    {
        [Fact]
        public async Task GetCurrent_ShouldReturnMappedResult()
        {
            var userId = NewId();

            var goal = new UserHealthGoal
            {
                UserId = userId,
                HealthGoalId = NewId(),
                StartedAtUtc = DateTime.UtcNow
            };

            UserHealthGoalRepositoryMock
                .Setup(r => r.GetActiveGoalByUserIdAsync(userId))
                .ReturnsAsync(goal);

            MapperMock
                .Setup(m => m.Map<UserHealthGoalResponse>(goal))
                .Returns(new UserHealthGoalResponse
                {
                    HealthGoalId = goal.HealthGoalId,
                    StartedAtUtc = goal.StartedAtUtc
                });

            var result = await Sut.GetCurrentGoalAsync(userId);

            Assert.NotNull(result);
            Assert.Equal(goal.HealthGoalId, result.HealthGoalId);

            UserHealthGoalRepositoryMock.VerifyAll();
            MapperMock.VerifyAll();
        }
    }
}
