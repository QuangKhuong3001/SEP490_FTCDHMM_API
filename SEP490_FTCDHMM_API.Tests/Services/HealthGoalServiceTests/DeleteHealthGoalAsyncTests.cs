using Moq;
using SEP490_FTCDHMM_API.Domain.Entities;
using SEP490_FTCDHMM_API.Shared.Exceptions;

namespace SEP490_FTCDHMM_API.Tests.Services.HealthGoalServiceTests
{
    public class DeleteHealthGoalAsyncTests : HealthGoalServiceTestBase
    {
        [Fact]
        public async Task Delete_ShouldThrow_WhenNotFound()
        {
            HealthGoalRepositoryMock
                .Setup(r => r.GetByIdAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<Func<IQueryable<HealthGoal>, IQueryable<HealthGoal>>>()))
                .ReturnsAsync((HealthGoal)null!);

            await Assert.ThrowsAsync<AppException>(() => Sut.DeleteHealthGoalAsync(NewId()));
        }

        [Fact]
        public async Task Delete_ShouldRemoveTargets_AndDeleteGoal()
        {
            var goal = new HealthGoal
            {
                Id = NewId(),
                Targets = new List<HealthGoalTarget>
                {
                    new HealthGoalTarget { Id = NewId(), NutrientId = NewId() }
                }
            };

            HealthGoalRepositoryMock
                .Setup(r => r.GetByIdAsync(
                    goal.Id,
                    It.IsAny<Func<IQueryable<HealthGoal>, IQueryable<HealthGoal>>>()))
                .ReturnsAsync(goal);

            HealthGoalTargetRepositoryMock
                .Setup(r => r.DeleteRangeAsync(goal.Targets))
                .Returns(Task.CompletedTask);

            HealthGoalRepositoryMock
                .Setup(r => r.DeleteAsync(goal))
                .Returns(Task.CompletedTask);

            CacheServiceMock
                .Setup(c => c.RemoveByPrefixAsync("health-goal"))
                .Returns(Task.CompletedTask);

            await Sut.DeleteHealthGoalAsync(goal.Id);

            HealthGoalRepositoryMock.VerifyAll();
            HealthGoalTargetRepositoryMock.VerifyAll();
            CacheServiceMock.VerifyAll();
        }

    }
}
