using Moq;
using SEP490_FTCDHMM_API.Application.Dtos.UserHealthMetricDtos;
using SEP490_FTCDHMM_API.Domain.Entities;
using SEP490_FTCDHMM_API.Domain.ValueObjects;
using SEP490_FTCDHMM_API.Shared.Exceptions;

namespace SEP490_FTCDHMM_API.Tests.Services.UserHealthMetricServiceTests
{
    public class UpdateHealthMetricAsyncTests : UserHealthMetricServiceTestBase
    {
        [Fact]
        public async Task UpdateHealthMetricAsync_ShouldThrow_WhenUserOrMetricNotFound()
        {
            var userId = Guid.NewGuid();
            var metricId = Guid.NewGuid();

            UserRepoMock
                .Setup(r => r.GetByIdAsync(userId, null))
                .ReturnsAsync((AppUser)null!);

            MetricRepoMock
                .Setup(r => r.GetByIdAsync(metricId, null))
                .ReturnsAsync((UserHealthMetric)null!);

            await Assert.ThrowsAsync<AppException>(() =>
                Sut.UpdateHealthMetricAsync(userId, metricId, new UpdateUserHealthMetricRequest()));
        }

        [Fact]
        public async Task UpdateHealthMetricAsync_ShouldThrow_WhenNotOwner()
        {
            var userId = Guid.NewGuid();
            var metricId = Guid.NewGuid();

            var metric = new UserHealthMetric
            {
                Id = metricId,
                UserId = Guid.NewGuid()
            };

            var user = new AppUser
            {
                Id = userId,
                DateOfBirth = new DateTime(2000, 1, 1),
                Gender = Gender.Male,
                ActivityLevel = ActivityLevel.Moderate
            };

            MetricRepoMock
                .Setup(r => r.GetByIdAsync(metricId, null))
                .ReturnsAsync(metric);

            UserRepoMock
                .Setup(r => r.GetByIdAsync(userId, null))
                .ReturnsAsync(user);

            await Assert.ThrowsAsync<AppException>(() =>
                Sut.UpdateHealthMetricAsync(userId, metricId, new UpdateUserHealthMetricRequest()));
        }

        [Fact]
        public async Task UpdateHealthMetricAsync_ShouldUpdateMetric()
        {
            var userId = Guid.NewGuid();
            var metricId = Guid.NewGuid();

            var metric = new UserHealthMetric
            {
                Id = metricId,
                UserId = userId,
                HeightCm = 170,
                WeightKg = 70
            };

            var user = new AppUser
            {
                Id = userId,
                DateOfBirth = new DateTime(1999, 1, 1),
                Gender = Gender.Female,
                ActivityLevel = ActivityLevel.Light
            };

            var request = new UpdateUserHealthMetricRequest
            {
                HeightCm = 165,
                WeightKg = 60,
                BodyFatPercent = 20,
                MuscleMassKg = 25,
                Notes = "Updated"
            };

            MetricRepoMock
                .Setup(r => r.GetByIdAsync(metricId, null))
                .ReturnsAsync(metric);

            UserRepoMock
                .Setup(r => r.GetByIdAsync(userId, null))
                .ReturnsAsync(user);

            MetricRepoMock
                .Setup(r => r.UpdateAsync(metric))
                .Returns(Task.CompletedTask)
                .Verifiable();

            await Sut.UpdateHealthMetricAsync(userId, metricId, request);

            MetricRepoMock.Verify(r => r.UpdateAsync(metric), Times.Once);
        }
    }
}
