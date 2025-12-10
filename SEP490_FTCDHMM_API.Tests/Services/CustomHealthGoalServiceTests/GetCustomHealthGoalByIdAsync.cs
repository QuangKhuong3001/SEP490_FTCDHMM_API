using Moq;
using SEP490_FTCDHMM_API.Application.Dtos.HealthGoalDtos;
using SEP490_FTCDHMM_API.Domain.Entities;
using SEP490_FTCDHMM_API.Shared.Exceptions;

namespace SEP490_FTCDHMM_API.Tests.Services.CustomHealthGoalServiceTests
{
    public class GetCustomHealthGoalByIdAsyncTests : CustomHealthGoalServiceTestBase
    {
        [Fact]
        public async Task GetCustomHealthGoalByIdAsync_ShouldThrow_WhenNotFound()
        {
            CustomHealthGoalRepositoryMock
                .Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<Func<IQueryable<CustomHealthGoal>, IQueryable<CustomHealthGoal>>>()))
                .ReturnsAsync((CustomHealthGoal?)null);

            await Assert.ThrowsAsync<AppException>(() =>
                Sut.GetCustomHealthGoalByIdAsync(Guid.NewGuid(), Guid.NewGuid()));
        }

        [Fact]
        public async Task GetCustomHealthGoalByIdAsync_ShouldThrow_WhenForbidden()
        {
            var goal = new CustomHealthGoal { UserId = Guid.NewGuid() };

            CustomHealthGoalRepositoryMock
                .Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<Func<IQueryable<CustomHealthGoal>, IQueryable<CustomHealthGoal>>>()))
                .ReturnsAsync(goal);

            await Assert.ThrowsAsync<AppException>(() =>
                Sut.GetCustomHealthGoalByIdAsync(Guid.NewGuid(), Guid.NewGuid()));
        }

        [Fact]
        public async Task GetCustomHealthGoalByIdAsync_ShouldReturnSuccessfully()
        {
            var userId = Guid.NewGuid();

            var goal = new CustomHealthGoal
            {
                UserId = userId,
                Targets = new List<HealthGoalTarget>()
            };

            CustomHealthGoalRepositoryMock
                .Setup(r => r.GetByIdAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<Func<IQueryable<CustomHealthGoal>, IQueryable<CustomHealthGoal>>>()))
                .ReturnsAsync(goal);

            MapperMock
                .Setup(m => m.Map<HealthGoalResponse>(goal))
                .Returns(new HealthGoalResponse());

            var result = await Sut.GetCustomHealthGoalByIdAsync(userId, Guid.NewGuid());

            Assert.NotNull(result);
        }

    }
}
