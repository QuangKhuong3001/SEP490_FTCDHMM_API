using Moq;
using SEP490_FTCDHMM_API.Application.Dtos.HealthGoalDtos;
using SEP490_FTCDHMM_API.Domain.Entities;
using SEP490_FTCDHMM_API.Shared.Exceptions;

namespace SEP490_FTCDHMM_API.Tests.Services.HealthGoalServiceTests
{
    public class GetHealthGoalByIdAsyncTests : HealthGoalServiceTestBase
    {
        [Fact]
        public async Task Get_ShouldThrow_WhenNotFound()
        {
            HealthGoalRepositoryMock
                .Setup(r => r.GetByIdAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<Func<IQueryable<HealthGoal>, IQueryable<HealthGoal>>>()))
                .ReturnsAsync((HealthGoal)null!);

            await Assert.ThrowsAsync<AppException>(() => Sut.GetHealthGoalByIdAsync(NewId()));
        }

        [Fact]
        public async Task Get_ShouldReturnMappedResult()
        {
            var id = NewId();
            var goal = new HealthGoal { Id = id };

            HealthGoalRepositoryMock
                .Setup(r => r.GetByIdAsync(
                    id,
                    It.IsAny<Func<IQueryable<HealthGoal>, IQueryable<HealthGoal>>>()))
                .ReturnsAsync(goal);

            MapperMock
                .Setup(m => m.Map<HealthGoalResponse>(goal))
                .Returns(new HealthGoalResponse { Id = id });

            CacheServiceMock
                .Setup(c => c.SetAsync(
                    $"health-goal:system:detail:{id}",
                    It.IsAny<HealthGoalResponse>(),
                    It.IsAny<TimeSpan>()))
                .Returns(Task.CompletedTask);

            var result = await Sut.GetHealthGoalByIdAsync(id);

            Assert.NotNull(result);
            Assert.Equal(id, result.Id);

            HealthGoalRepositoryMock.VerifyAll();
            MapperMock.VerifyAll();
            CacheServiceMock.VerifyAll();
        }
    }
}
