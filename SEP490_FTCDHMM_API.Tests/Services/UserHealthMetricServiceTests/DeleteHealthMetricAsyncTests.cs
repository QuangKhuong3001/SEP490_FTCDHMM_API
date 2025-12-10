using Moq;
using SEP490_FTCDHMM_API.Domain.Entities;
using SEP490_FTCDHMM_API.Shared.Exceptions;

namespace SEP490_FTCDHMM_API.Tests.Services.UserHealthMetricServiceTests
{
    public class DeleteHealthMetricAsyncTests : UserHealthMetricServiceTestBase
    {
        [Fact]
        public async Task DeleteHealthMetricAsync_ShouldThrow_WhenNotFound()
        {
            MetricRepoMock
                .Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), null))
                .ReturnsAsync((UserHealthMetric)null!);

            await Assert.ThrowsAsync<AppException>(() =>
                Sut.DeleteHealthMetricAsync(Guid.NewGuid(), Guid.NewGuid()));
        }

        [Fact]
        public async Task DeleteHealthMetricAsync_ShouldThrow_WhenNotOwner()
        {
            var metric = new UserHealthMetric
            {
                Id = Guid.NewGuid(),
                UserId = Guid.NewGuid()
            };

            MetricRepoMock
                .Setup(r => r.GetByIdAsync(metric.Id, null))
                .ReturnsAsync(metric);

            await Assert.ThrowsAsync<AppException>(() =>
                Sut.DeleteHealthMetricAsync(Guid.NewGuid(), metric.Id));
        }

        [Fact]
        public async Task DeleteHealthMetricAsync_ShouldDelete()
        {
            var userId = Guid.NewGuid();

            var metric = new UserHealthMetric
            {
                Id = Guid.NewGuid(),
                UserId = userId
            };

            MetricRepoMock
                .Setup(r => r.GetByIdAsync(metric.Id, null))
                .ReturnsAsync(metric);

            MetricRepoMock
                .Setup(r => r.DeleteAsync(metric))
                .Returns(Task.CompletedTask)
                .Verifiable();

            await Sut.DeleteHealthMetricAsync(userId, metric.Id);

            MetricRepoMock.Verify(r => r.DeleteAsync(metric), Times.Once);
        }
    }
}
