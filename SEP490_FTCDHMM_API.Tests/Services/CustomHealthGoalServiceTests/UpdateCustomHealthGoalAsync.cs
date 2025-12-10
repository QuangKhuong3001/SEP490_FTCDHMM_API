using Moq;
using SEP490_FTCDHMM_API.Application.Dtos.CustomHealthGoalDtos;
using SEP490_FTCDHMM_API.Application.Dtos.NutrientDtos.NutrientTarget;
using SEP490_FTCDHMM_API.Domain.Entities;
using SEP490_FTCDHMM_API.Shared.Exceptions;

namespace SEP490_FTCDHMM_API.Tests.Services.CustomHealthGoalServiceTests
{
    public class UpdateCustomHealthGoalAsyncTests : CustomHealthGoalServiceTestBase
    {
        [Fact]
        public async Task UpdateCustomHealthGoalAsync_ShouldThrow_WhenGoalNotFound()
        {
            CustomHealthGoalRepositoryMock
                .Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<Func<IQueryable<CustomHealthGoal>, IQueryable<CustomHealthGoal>>>()))
                .ReturnsAsync((CustomHealthGoal?)null);

            var req = new UpdateCustomHealthGoalRequest();

            await Assert.ThrowsAsync<AppException>(() =>
                Sut.UpdateCustomHealthGoalAsync(Guid.NewGuid(), Guid.NewGuid(), req));
        }

        [Fact]
        public async Task UpdateCustomHealthGoalAsync_ShouldThrow_WhenForbidden()
        {
            var goal = new CustomHealthGoal { UserId = Guid.NewGuid() };

            CustomHealthGoalRepositoryMock
                .Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<Func<IQueryable<CustomHealthGoal>, IQueryable<CustomHealthGoal>>>()))
                .ReturnsAsync(goal);

            var req = new UpdateCustomHealthGoalRequest();

            await Assert.ThrowsAsync<AppException>(() =>
                Sut.UpdateCustomHealthGoalAsync(Guid.NewGuid(), Guid.NewGuid(), req));
        }

        [Fact]
        public async Task UpdateCustomHealthGoalAsync_ShouldUpdateSuccessfully()
        {
            var userId = Guid.NewGuid();

            var goal = new CustomHealthGoal
            {
                UserId = userId,
                Targets = new List<HealthGoalTarget>
                {
                    new HealthGoalTarget { NutrientId = Guid.NewGuid() }
                }
            };

            CustomHealthGoalRepositoryMock
                .Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<Func<IQueryable<CustomHealthGoal>, IQueryable<CustomHealthGoal>>>()))
                .ReturnsAsync(goal);

            NutrientRepositoryMock
                .Setup(r => r.IdsExistAsync(It.IsAny<List<Guid>>()))
                .ReturnsAsync(true);

            CustomHealthGoalRepositoryMock
                .Setup(r => r.UpdateAsync(goal))
                .Returns(Task.CompletedTask)
                .Verifiable();

            var req = new UpdateCustomHealthGoalRequest
            {
                Targets =
                {
                    new NutrientTargetRequest
                    {
                        NutrientId = Guid.NewGuid(),
                        TargetType = "Absolute",
                        MinValue = 10,
                        MaxValue = 20,
                        Weight = 1
                    }
                }
            };

            await Sut.UpdateCustomHealthGoalAsync(userId, Guid.NewGuid(), req);

            CustomHealthGoalRepositoryMock.Verify(r => r.UpdateAsync(goal), Times.Once);
        }
    }
}
