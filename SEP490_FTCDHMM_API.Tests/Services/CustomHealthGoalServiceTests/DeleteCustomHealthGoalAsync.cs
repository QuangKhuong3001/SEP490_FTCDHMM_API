using Moq;
using SEP490_FTCDHMM_API.Domain.Entities;
using SEP490_FTCDHMM_API.Shared.Exceptions;

namespace SEP490_FTCDHMM_API.Tests.Services.CustomHealthGoalServiceTests
{
    public class DeleteCustomHealthGoalAsyncTests : CustomHealthGoalServiceTestBase
    {
        [Fact]
        public async Task DeleteCustomHealthGoalAsync_ShouldThrow_WhenNotFound()
        {
            CustomHealthGoalRepositoryMock
                .Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<Func<IQueryable<CustomHealthGoal>, IQueryable<CustomHealthGoal>>>()))
                .ReturnsAsync((CustomHealthGoal?)null);

            await Assert.ThrowsAsync<AppException>(() =>
                Sut.DeleteCustomHealthGoalAsync(Guid.NewGuid(), Guid.NewGuid()));
        }

        [Fact]
        public async Task DeleteCustomHealthGoalAsync_ShouldThrow_WhenForbidden()
        {
            var goal = new CustomHealthGoal { UserId = Guid.NewGuid() };

            CustomHealthGoalRepositoryMock
                .Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<Func<IQueryable<CustomHealthGoal>, IQueryable<CustomHealthGoal>>>()))
                .ReturnsAsync(goal);

            await Assert.ThrowsAsync<AppException>(() =>
                Sut.DeleteCustomHealthGoalAsync(Guid.NewGuid(), Guid.NewGuid()));
        }

        [Fact]
        public async Task DeleteCustomHealthGoalAsync_ShouldDeleteSuccessfully()
        {
            var userId = Guid.NewGuid();

            var goal = new CustomHealthGoal
            {
                UserId = userId,
                Targets = new List<HealthGoalTarget>()
            };

            CustomHealthGoalRepositoryMock
                .Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<Func<IQueryable<CustomHealthGoal>, IQueryable<CustomHealthGoal>>>()))
                .ReturnsAsync(goal);

            CustomHealthGoalRepositoryMock
                .Setup(r => r.DeleteAsync(goal))
                .Returns(Task.CompletedTask)
                .Verifiable();

            await Sut.DeleteCustomHealthGoalAsync(userId, Guid.NewGuid());

            CustomHealthGoalRepositoryMock.Verify(r => r.DeleteAsync(goal), Times.Once);
        }
    }
}
